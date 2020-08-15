﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnTTSharp.Entities.Helpers;
using EnTTSharp.Entities.Pools;

namespace EnTTSharp.Entities
{
    public partial class EntityRegistry<TEntityKey> : IEntityViewFactory<TEntityKey>,
                                                      IEntityPoolAccess<TEntityKey>,
                                                      IEntityComponentRegistry<TEntityKey>
        where TEntityKey : IEntityKey
    {
        readonly EqualityComparer<TEntityKey> equalityComparer;
        readonly List<TEntityKey> entities;
        readonly List<IPool<TEntityKey>> pools;
        readonly Dictionary<Type, IComponentRegistration> componentIndex;
        readonly Dictionary<Type, int> tagIndex;
        readonly List<Attachment> tags;
        readonly Dictionary<Type, IEntityView<TEntityKey>> views;
        readonly Func<byte, int, TEntityKey> entityKeyFactory;
        int next;
        int available;

        public int MaxAge { get; }


        public EntityRegistry(int maxAge, Func<byte, int, TEntityKey> entityKeyFactory)
        {
            if (maxAge < 2)
            {
                throw new ArgumentException();
            }
            MaxAge = maxAge;
            this.entityKeyFactory = entityKeyFactory ?? throw new ArgumentNullException(nameof(entityKeyFactory));
            equalityComparer = EqualityComparer<TEntityKey>.Default;
            componentIndex = new Dictionary<Type, IComponentRegistration>();
            entities = new List<TEntityKey>();
            pools = new List<IPool<TEntityKey>>();
            tagIndex = new Dictionary<Type, int>();
            tags = new List<Attachment>();
            views = new Dictionary<Type, IEntityView<TEntityKey>>();
        }

        public event EventHandler<TEntityKey> BeforeEntityDestroyed;

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

        IEnumerator<TEntityKey> IEnumerable<TEntityKey>.GetEnumerator()
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

        IComponentRegistration<TEntityKey, TComponent> GetRegistration<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return (IComponentRegistration<TEntityKey, TComponent>)reg;
            }

            throw new ArgumentException($"Unknown registration for type {typeof(TComponent)}");
        }

        internal int CountComponents<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            return pools[idx].Count;
        }

        public IPool<TEntityKey, TComponent> GetPool<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            return (IPool<TEntityKey, TComponent>)pools[idx];
        }

        public IPool<TEntityKey, TComponent> Register<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, () => new TComponent());
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        public IPool<TEntityKey, TComponent> RegisterFlag<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create<TEntityKey, TComponent>(componentIndex.Count, this);
            var pool = PoolFactory.CreateFlagPool(new TComponent(), registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        public IPool<TEntityKey, TComponent> RegisterFlag<TComponent>(TComponent sharedData)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create<TEntityKey, TComponent>(componentIndex.Count, this);
            var pool = PoolFactory.CreateFlagPool(sharedData, registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        void IEntityComponentRegistry<TEntityKey>.Register<TComponent>(Func<TComponent> constructorFn,
                                                                       Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn)
        {
            Register(constructorFn, destructorFn);
        }

        public IPool<TEntityKey, TComponent>
            Register<TComponent>(Func<TComponent> con,
                                 Action<TEntityKey, EntityRegistry<TEntityKey>, TComponent> destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, con, destructor);
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, pool);
            return pool;
        }

        void IEntityComponentRegistry<TEntityKey>.RegisterNonConstructable<TComponent>
            (Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent> destructorFn)
        {
            RegisterNonConstructable(destructorFn);
        }

        public IPool<TEntityKey, TComponent> RegisterNonConstructable<TComponent>(
            Action<TEntityKey, EntityRegistry<TEntityKey>, TComponent> destructor = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, destructor);
            var pool = PoolFactory.Create(registration);
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

        public bool IsValid(TEntityKey key)
        {
            return equalityComparer.Equals(entities[key.Key], key);
        }

        public int StoredVersion(EntityKey key)
        {
            return entities[key.Key].Age;
        }

        public TEntityKey Create()
        {
            if (available > 0)
            {
                // for empty slots, entities contains the pointer to the next empty element
                // this is filled in during destroy ...
                var entityKey = next;
                var nextEmpty = entities[next];
                var entity = entityKeyFactory(nextEmpty.Age, entityKey);
                entities[next] = entity;
                next = nextEmpty.Key;
                available -= 1;
                return entity;
            }
            else
            {
                var entity = entityKeyFactory(1, entities.Count);
                entities.Add(entity);
                return entity;
            }
        }

        public void Destroy(TEntityKey entity)
        {
            AssertValid(entity);

            BeforeEntityDestroyed?.Invoke(this, entity);

            var entt = entity.Key;
            var node = entityKeyFactory(RollingAgeIncrement(entity.Age), available > 0 ? next : entt + 1);

            entities[entt] = node;
            next = entt;
            available += 1;

            foreach (var pool in pools)
            {
                pool.Remove(entity);
            }
        }

        public bool HasTag<TTag>()
        {
            if (!tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                return false;
            }

            return tags[idx].Tag != null;
        }

        public void AttachTag<TTag>(TEntityKey entity)
        {
            var tag = GetRegistration<TTag>().Create();
            AttachTag(entity, tag);
        }

        public void AttachTag<TTag>(TEntityKey entity, in TTag tag)
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
                tags.Add(new Attachment(entity, tag));
            }
            else
            {
                tags[idx] = new Attachment(entity, tag);
            }
        }

        void AssertValid(TEntityKey entity)
        {
            if (!IsValid(entity))
            {
                throw new ArgumentException($"Key {entity} is not valid in this registry.");
            }
        }

        /// <summary>
        ///  Synchronizes the state of the registry with data received from the
        ///  snapshot loader. This will insert keys by using internal knowledge
        ///  of the data structured used.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="destroyed"></param>
        public void AssureEntityState(TEntityKey entity, bool destroyed)
        {
            var entt = entity.Key;
            while (entities.Count <= entt)
            {
                entities.Add(entityKeyFactory(0, entities.Count));
            }

            entities[entt] = entity;
            if (destroyed)
            {
                Destroy(entity);
                entities[entt] = entityKeyFactory(entity.Age, entities[entt].Key);
            }
        }

        public void RemoveTag<TTag>()
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var e = tags[idx];
                if (e.Tag != null)
                {
                    GetRegistration<TTag>().Destruct(e.Entity, (TTag)e.Tag);
                }

                tags[idx] = default(Attachment);
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

                return (TTag)tagRaw;
            }

            throw new ArgumentException();
        }

        public bool TryGetTag<TTag>(out TEntityKey entity, out TTag tag)
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                entity = att.Entity;
                tag = (TTag)att.Tag;
                return true;
            }

            entity = default;
            tag = default;
            return false;
        }

        public TEntityKey GetTaggedEntity<TTag>()
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

        public TComponent AssignComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();

            var component = GetRegistration<TComponent>().Create();
            AssignComponent(entity, in component);
            return component;
        }

        public void AssignComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            GetPool<TComponent>().Add(entity, in c);
        }

        public void RemoveComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            GetPool<TComponent>().Remove(entity);
        }

        public bool HasComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            return GetPool<TComponent>().Contains(entity);
        }

        public bool GetComponent<TComponent>(TEntityKey entity, out TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            return GetPool<TComponent>().TryGet(entity, out c);
        }

        public TComponent AssignOrReplace<TComponent>(TEntityKey entity)
        {
            var component = GetRegistration<TComponent>().Create();
            AssignOrReplace(entity, in component);
            return component;
        }

        public void AssignOrReplace<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            var pool = GetPool<TComponent>();
            if (!pool.WriteBack(entity, in c))
            {
                pool.Add(entity, in c);
            }
        }

        public bool Contains(TEntityKey e)
        {
            return IsValid(e);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBack<TComponent>(TEntityKey entity, in TComponent data)
        {
            AssertValid(entity);
            GetPool<TComponent>().WriteBack(entity, in data);
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity)
        {
            return ReplaceComponent(entity, GetRegistration<TComponent>().Create());
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);
            return GetPool<TComponent>().WriteBack(entity, in c);
        }

        public void Sort<TComponent>(IComparer<TComponent> comparator)
        {
            var pool = GetPool<TComponent>();
            if (pool is ISortableCollection<TComponent> sortablePool)
            {
                sortablePool.HeapSort(comparator);
            }
        }

        public void Respect<TComponentTo, TComponentFrom>()
        {
            var pool = GetPool<TComponentTo>();
            pool.Respect(GetPool<TComponentFrom>());
        }

        public void ResetComponent<TComponent>()
        {
            GetPool<TComponent>().RemoveAll();
        }

        public void Reset()
        {
            var l = EntityKeyListPool<TEntityKey>.Reserve(GetEnumerator(), Count);
            foreach (var last in l)
            {
                if (IsValid(last))
                {
                    Destroy(last);
                }
            }

            EntityKeyListPool<TEntityKey>.Release(l);

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

        public void Reset(TEntityKey entity)
        {
            foreach (var pool in pools)
            {
                if (pool.Contains(entity))
                {
                    pool.Remove(entity);
                }
            }
        }

        public bool IsOrphan(TEntityKey e)
        {
            AssertValid(e);
            var orphan = true;
            foreach (var pool in pools)
            {
                orphan &= !pool.Contains(e);
            }

            foreach (var tag in tags)
            {
                orphan &= !equalityComparer.Equals(tag.Entity, e);
            }

            return orphan;
        }

        public EntityKeyEnumerator GetEnumerator()
        {
            return new EntityKeyEnumerator(entities);
        }

        byte RollingAgeIncrement(byte value)
        {
            value += 1;
            if (value == MaxAge)
            {
                return 1;
            }

            return value;
        }

        readonly struct Attachment
        {
            public readonly TEntityKey Entity;
            public readonly object Tag;

            public Attachment(TEntityKey entity, object tag)
            {
                Entity = entity;
                Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            }
        }

        public struct EntityKeyEnumerator : IEnumerator<TEntityKey>
        {
            readonly List<TEntityKey> contents;

            int index;
            TEntityKey current;

            internal EntityKeyEnumerator(List<TEntityKey> widget) : this()
            {
                contents = widget;
                index = -1;
                current = default;
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

                current = default;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default;
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public TEntityKey Current
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