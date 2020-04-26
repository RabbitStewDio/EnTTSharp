using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EnttSharp.Entities
{
    public partial class EntityRegistry : IEnumerable<EntityKey>, IEntityViewFactory
    {
        readonly List<EntityKey> entities;
        readonly List<SparseSet> pools;
        readonly Dictionary<Type, IComponentRegistration> componentIndex;
        readonly Dictionary<Type, int> tagIndex;
        readonly List<Attaching> tags;
        readonly Dictionary<Type, IEntityView> views;

        int next;
        int available;

        public EntityRegistry()
        {
            componentIndex = new Dictionary<Type, IComponentRegistration>();
            entities = new List<EntityKey>();
            pools = new List<SparseSet>();
            tagIndex = new Dictionary<Type, int>();
            tags = new List<Attaching>();
            views = new Dictionary<Type, IEntityView>();
        }

        public int Count
        {
            get { return entities.Count - available; }
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<EntityKey> IEnumerable<EntityKey>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool IsManaged<TComponent>()
        {
            return componentIndex.ContainsKey(typeof(TComponent));
        }

        int ManagedIndex<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return reg.Index;
            }

            throw new ArgumentException($"Unknown registration for type {typeof(TComponent)}");
        }

        IComponentRegistration<TComponent> GetRegistration<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return (IComponentRegistration<TComponent>) reg;
            }

            throw new ArgumentException($"Unknown registration for type {typeof(TComponent)}");
        }

        internal int CountComponents<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            return pools[idx].Count;
        }

        public Pools.Pool<TComponent> GetPool<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            return (Pools.Pool<TComponent>) pools[idx];
        }

        public Pools.Pool<TComponent> Register<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, () => new TComponent());
            var pool = Pools.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        public Pools.Pool<TComponent> Register<TComponent>(Func<TComponent> con,
                                                           Action<EntityKey, EntityRegistry, TComponent> destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, con, destructor);
            var pool = Pools.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        public Pools.Pool<TComponent> RegisterNonConstructable<TComponent>(
            Action<EntityKey, EntityRegistry, TComponent> destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, destructor);
            var pool = Pools.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        public void Reserve<TComponent>(int capacity)
        {
            GetPool<TComponent>().Reserve(capacity);
        }

        public bool IsPoolEmpty<TComponent>()
        {
            return GetPool<TComponent>().Count == 0;
        }

        public bool IsValid(EntityKey key)
        {
            return entities[key.Key] == key;
        }

        public int StoredVersion(EntityKey key)
        {
            return entities[key.Key].Age;
        }

        public EntityKey Create(uint data = 0)
        {
            if (available > 0)
            {
                // for empty slots, entities contains the pointer to the next empty element
                // this is filled in during destroy ...
                var entityKey = next;
                var nextEmpty = entities[next];
                var entity = new EntityKey(nextEmpty.Age, entityKey, data);
                entities[next] = entity;
                next = nextEmpty.Key;
                available -= 1;
                return entity;
            }
            else
            {
                var entity = new EntityKey(1, entities.Count, data);
                entities.Add(entity);
                return entity;
            }
        }

        public void Destroy(EntityKey entity)
        {
            AssertValid(entity);

            BeforeEntityDestroyed?.Invoke(this, entity);

            var entt = entity.Key;
            var node = new EntityKey(RollingAgeIncrement(entity.Age), available > 0 ? next : entt + 1);

            entities[entt] = node;
            next = entt;
            available += 1;

            foreach (var pool in pools)
            {
                pool.Remove(entity);
            }
        }

        public event EventHandler<EntityKey> BeforeEntityDestroyed;

        public bool HasTag<TTag>()
        {
            if (!tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                return false;
            }

            return tags[idx].Tag != null;
        }

        public void AttachTag<TTag>(EntityKey entity)
        {
            var tag = GetRegistration<TTag>().Create();
            AttachTag(entity, tag);
        }

        public void AttachTag<TTag>(EntityKey entity, in TTag tag)
        {
            AssertValid(entity);

            if (HasTag<TTag>())
            {
                throw new ArgumentException();
            }

            if (!tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                idx = tagIndex.Count;
                tagIndex[typeof(TTag)] = idx;
                tags.Add(new Attaching(entity, tag));
            }
            else
            {
                tags[idx] = new Attaching(entity, tag);
            }
        }

        void AssertValid(EntityKey entity)
        {
            if (!IsValid(entity))
            {
                throw new ArgumentException($"Key {entity} is not valid in this registry.");
            }
        }

        /// <summary>
        ///  Synchronizes the state of the registry with data received from the snapshot loader.
        ///  This will insert keys by using internal knowledge of the data structured used.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destroyed"></param>
        public void AssureEntityState(EntityKey entity, bool destroyed)
        {
            var entt = entity.Key;
            while (entities.Count <= entt)
            {
                entities.Add(new EntityKey(0, entities.Count));
            }

            entities[entt] = entity;
            if (destroyed)
            {
                Destroy(entity);
                entities[entt] = new EntityKey(entity.Age, entities[entt].Key);
            }
        }

        public void RemoveTag<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var e = tags[idx];
                if (e.Tag != null)
                {
                    GetRegistration<TTag>().Destruct(e.Entity, (TTag) e.Tag);
                }

                tags[idx] = default(Attaching);
            }
        }

        public TTag GetTag<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                var tagRaw = att.Tag;
                if (tagRaw == null)
                {
                    throw new ArgumentException();
                }

                return (TTag) tagRaw;
            }

            throw new ArgumentException();
        }

        public bool TryGetTag<TTag>(out EntityKey entity, out TTag tag)
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                entity = att.Entity;
                tag = (TTag) att.Tag;
                return true;
            }

            entity = default(EntityKey);
            tag = default(TTag);
            return false;
        }

        public EntityKey GetTaggedEntity<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                var tagRaw = att.Tag;
                if (tagRaw == null)
                {
                    throw new ArgumentException($"No entity is tagged with type {typeof(TTag)}");
                }

                return att.Entity;
            }

            throw new ArgumentException($"No entity is tagged with type {typeof(TTag)}");
        }

        void AssertManaged<TComponent>()
        {
            if (!IsManaged<TComponent>())
            {
                throw new ArgumentException($"Unknown registration for type {typeof(TComponent)}");
            }
        }

        public TComponent AssignComponent<TComponent>(EntityKey entity)
        {
            AssertManaged<TComponent>();

            var component = GetRegistration<TComponent>().Create();
            AssignComponent(entity, in component);
            return component;
        }

        public void AssignComponent<TComponent>(EntityKey entity, in TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            GetPool<TComponent>().Add(entity, in c);
        }

        public void RemoveComponent<TComponent>(EntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            GetPool<TComponent>().Remove(entity);
        }

        public bool HasComponent<TComponent>(EntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            return GetPool<TComponent>().Contains(entity);
        }

        public bool GetComponent<TComponent>(EntityKey entity, out TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            return GetPool<TComponent>().TryGet(entity, out c);
        }

        public TComponent AssignOrReplace<TComponent>(EntityKey entity)
        {
            var component = GetRegistration<TComponent>().Create();
            AssignOrReplace(entity, in component);
            return component;
        }

        public void AssignOrReplace<TComponent>(EntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            var pool = GetPool<TComponent>();
            if (pool.Contains(entity))
            {
                pool.Replace(entity, in c);
            }
            else
            {
                pool.Add(entity, in c);
            }
        }

        public bool Contains(EntityKey e)
        {
            return IsValid(e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBack<TComponent>(EntityKey entity, in TComponent data)
        {
            AssertValid(entity);
            GetPool<TComponent>().WriteBack(entity, in data);
        }

        public bool ReplaceComponent<TComponent>(EntityKey entity)
        {
            return ReplaceComponent(entity, GetRegistration<TComponent>().Create());
        }

        public bool ReplaceComponent<TComponent>(EntityKey entity, in TComponent c)
        {
            AssertValid(entity);
            return GetPool<TComponent>().Replace(entity, in c);
        }

        public void Sort<TComponent>(IComparer<TComponent> comparator)
        {
            var pool = GetPool<TComponent>();
            pool.HeapSort(comparator);
        }

        public void Respect<TComponentTo, TComponentFrom>()
        {
            GetPool<TComponentTo>().Respect(GetPool<TComponentFrom>());
        }

        public void ResetComponent<TComponent>()
        {
            GetPool<TComponent>().RemoveAll();
        }

        public void Reset()
        {
            var l = EntityKeyListPool.Reserve(GetEnumerator(), Count);
            foreach (var last in l)
            {
                if (IsValid(last))
                {
                    Destroy(last);
                }
            }
            EntityKeyListPool.Release(l);

            if (!IsEmpty)
            {
                // someone is doing something silly, like creating new entities
                // during create. Nuke the beast from orbit ..

                foreach (var pool in pools)
                {
                    pool.RemoveAll();
                }
                entities.Clear();
                available = 0;
                next = default;
            }
        }

        public void Reset(EntityKey entity)
        {
            foreach (var pool in pools)
            {
                if (pool.Contains(entity))
                {
                    pool.Remove(entity);
                }
            }
        }

        public bool IsOrphan(EntityKey e)
        {
            AssertValid(e);
            var orphan = true;
            foreach (var pool in pools)
            {
                orphan &= !pool.Contains(e);
            }

            foreach (var tag in tags)
            {
                orphan &= tag.Entity != e;
            }

            return orphan;
        }

        public EntityKeyEnumerator GetEnumerator()
        {
            return new EntityKeyEnumerator(entities);
        }

        static byte RollingAgeIncrement(byte value)
        {
            value += 1;
            if (value == 16)
            {
                return 1;
            }

            return value;
        }

        struct Attaching
        {
            public readonly EntityKey Entity;
            public readonly object Tag;

            public Attaching(EntityKey entity, object tag)
            {
                Entity = entity;
                Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            }
        }

        public struct EntityKeyEnumerator : IEnumerator<EntityKey>
        {
            readonly List<EntityKey> contents;

            int index;
            EntityKey current;

            internal EntityKeyEnumerator(List<EntityKey> widget) : this()
            {
                contents = widget;
                index = -1;
                current = default(EntityKey);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                while (index + 1 < contents.Count)
                {
                    index += 1;
                    var c = contents[index];
                    if (c.Key == index)
                    {
                        current = contents[index];
                        return true;
                    }
                }

                current = default(EntityKey);
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default(EntityKey);
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public EntityKey Current
            {
                get
                {
                    if (index < 0 || index >= contents.Count)
                    {
                        throw new InvalidOperationException();
                    }

                    return current;
                }
            }
        }
    }
}