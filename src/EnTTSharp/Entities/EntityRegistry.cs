using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using EnTTSharp.Entities.Helpers;
using EnTTSharp.Entities.Pools;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities
{
    public partial class EntityRegistry<TEntityKey> : IEntityViewFactory<TEntityKey>,
                                                      IEntityPoolAccess<TEntityKey>,
                                                      IEntityComponentRegistry<TEntityKey>
        where TEntityKey : IEntityKey
    {
        readonly struct PoolEntry
        {
            readonly IPool<TEntityKey>? pool;

            public PoolEntry(IReadOnlyPool<TEntityKey> readonlyPool)
            {
                this.ReadonlyPool = readonlyPool ?? throw new ArgumentNullException(nameof(readonlyPool));
                this.pool = null;
            }

            public PoolEntry(IPool<TEntityKey> pool)
            {
                this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
                this.ReadonlyPool = pool;
            }

            public IReadOnlyPool<TEntityKey> ReadonlyPool { get; }

            public bool TryGetPool([MaybeNullWhen(false)] out IPool<TEntityKey> poolResult)
            {
                poolResult = this.pool;
                return poolResult != null;
            }

            public override string ToString()
            {
                return $"(Pool: {pool})";
            }
        }

        readonly ILogger logger = LogHelper.ForContext<EntityRegistry<TEntityKey>>();
        readonly EqualityComparer<TEntityKey> equalityComparer;
        readonly List<TEntityKey> entities;
        readonly List<PoolEntry> pools;
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
            pools = new List<PoolEntry>();
            tagIndex = new Dictionary<Type, int>();
            tags = new List<Attachment>();
            views = new Dictionary<Type, IEntityView<TEntityKey>>();
        }

        public event EventHandler<TEntityKey>? BeforeEntityDestroyed;

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

            return -1;
        }

        IComponentRegistration<TEntityKey, TComponent> GetRegistration<TComponent>()
        {
            if (componentIndex.TryGetValue(typeof(TComponent), out var reg))
            {
                return (IComponentRegistration<TEntityKey, TComponent>)reg;
            }

            ThrowInvalidRegistrationError(typeof(TComponent));
            return default;
        }

        [DoesNotReturn]
        void ThrowInvalidRegistrationError(Type t)
        {
            throw new ArgumentException($"Unknown registration at EntityRegistry<{typeof(TEntityKey)}> for component type {t}");
        }

        internal int CountComponents<TComponent>()
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return -1;
            }

            return pools[idx].ReadonlyPool.Count;
        }

        public IReadOnlyPool<TEntityKey, TComponent> GetPool<TComponent>()
        {
            if (!TryGetPool<TComponent>(out var p))
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return default;
            }

            return p;
        }

        public bool TryGetPool<TComponent>([MaybeNullWhen(false)] out IReadOnlyPool<TEntityKey, TComponent> pool)
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                pool = default;
                return false;
            }

            if (pools[idx].TryGetPool(out var maybePool) && maybePool is IReadOnlyPool<TEntityKey, TComponent> p)
            {
                pool = p;
                return true;
            }

            pool = default;
            return false;
        }

        public IPool<TEntityKey, TComponent> GetWritablePool<TComponent>()
        {
            if (!TryGetWritablePool<TComponent>(out var p))
            {
                ThrowInvalidRegistrationError(typeof(TComponent));
                return default;
            }

            return p;
        }

        public bool TryGetWritablePool<TComponent>([MaybeNullWhen(false)] out IPool<TEntityKey, TComponent> pool)
        {
            var idx = ManagedIndex<TComponent>();
            if (idx == -1)
            {
                pool = default;
                return false;
            }

            if (pools[idx].TryGetPool(out var p) && p is IPool<TEntityKey, TComponent> pp)
            {
                pool = pp;
                return true;
            }

            pool = default;
            return false;
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
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }

        public IPool<TEntityKey, TComponent> RegisterFlag<TComponent>() where TComponent : new()
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, () => new TComponent());
            var pool = PoolFactory.CreateFlagPool(new TComponent(), registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
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
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }

        public IReadOnlyPool<TEntityKey, Not<TComponent>> RegisterNonExistingFlag<TComponent>()
        {
            if (IsManaged<Not<TComponent>>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            if (!IsManaged<TComponent>())
            {
                throw new ArgumentException($"Require component registration of base type {typeof(TComponent)}");
            }

            var registration = ComponentRegistration.Create<TEntityKey, Not<TComponent>>(componentIndex.Count, this);
            var basePool = GetPool<TComponent>();
            var pool = new NotPool<TEntityKey, TComponent>(this, basePool);
            componentIndex[typeof(Not<TComponent>)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
            return pool;
        }

        void IEntityComponentRegistry<TEntityKey>.RegisterFlag<TComponent>()
        {
            RegisterFlag(default(TComponent));
        }

        public void Register<TComponent>(Func<TComponent> constructorFn,
                                         Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, constructorFn, destructorFn);
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
        }

        public void RegisterDerived<TSourceComponent, TComponent>(Func<TEntityKey, TSourceComponent, TComponent> fn)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            if (!IsManaged<TSourceComponent>())
            {
                throw new ArgumentException("Invalid source component registration");
            }

            var sourcePool = GetPool<TSourceComponent>();
            var registration = ComponentRegistration.Create<TEntityKey, TComponent>(componentIndex.Count, this);
            var pool = new DerivedValuePool<TEntityKey, TSourceComponent, TComponent>(sourcePool, fn);
            pools.StoreAt(registration.Index, new PoolEntry(pool));
        } 
        
        public void RegisterNonConstructable<TComponent>(Action<TEntityKey, IEntityViewControl<TEntityKey>, TComponent>? destructorFn = null)
        {
            if (IsManaged<TComponent>())
            {
                throw new ArgumentException("Duplicate registration");
            }

            var registration = ComponentRegistration.Create(componentIndex.Count, this, destructorFn);
            var pool = PoolFactory.Create(registration);
            componentIndex[typeof(TComponent)] = registration;
            pools.StoreAt(registration.Index, new PoolEntry(pool));
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
            if (key.Key < 0 || key.Key >= entities.Count)
            {
                return false;
            }

            return equalityComparer.Equals(entities[key.Key], key);
        }

        internal int StoredVersion(EntityKey key)
        {
            return entities[key.Key].Age;
        }

        public event EventHandler<TEntityKey>? EntityCreated;

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
                EntityCreated?.Invoke(this, entity);
                return entity;
            }
            else
            {
                var entity = entityKeyFactory(1, entities.Count);
                entities.Add(entity);
                EntityCreated?.Invoke(this, entity);
                return entity;
            }
        }

        public void Destroy(TEntityKey entity)
        {
            AssertValid(entity);

            BeforeEntityDestroyed?.Invoke(this, entity);
            logger.Verbose("Destroying {Entity}", entity);

            var entt = entity.Key;
            var node = entityKeyFactory(RollingAgeIncrement(entity.Age), available > 0 ? next : entt + 1);

            foreach (var pool in pools)
            {
                if (pool.TryGetPool(out var p))
                {
                    logger.Verbose("- Removing component from pool [{Pool}] entry {Entity}", p, entity);
                    p.Remove(entity);
                }
            }

            entities[entt] = node;
            next = entt;
            available += 1;
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

        public void AttachTag<TTag>(TEntityKey entity, in TTag? tag)
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

                tags[idx] = default;
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

        public bool TryGetTag<TTag>([MaybeNullWhen(false)] out TEntityKey entity, [MaybeNullWhen(false)] out TTag tag)
        {
            if (tagIndex.TryGetValue(typeof(TTag), out var idx))
            {
                var att = tags[idx];
                if (att.Tag is TTag typedTag)
                {
                    tag = typedTag;
                    entity = att.Entity;
                    return true;
                }
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
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.Add(entity, in c);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }

        public void RemoveComponent<TComponent>(TEntityKey entity)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.Remove(entity);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }

        public bool HasComponent<TComponent>(TEntityKey entity)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            if (TryGetPool<TComponent>(out var pool))
            {
                return pool.Contains(entity);
            }

            AssertManaged<TComponent>();
            return false;
        }

        public bool GetComponent<TComponent>(TEntityKey entity, [MaybeNullWhen(false)] out TComponent c)
        {
            AssertManaged<TComponent>();
            AssertValid(entity);
            if (TryGetPool<TComponent>(out var pool))
            {
                return pool.TryGet(entity, out c);
            }

            AssertManaged<TComponent>();
            c = default;
            return false;
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

            if (TryGetWritablePool<TComponent>(out var p))
            {
                if (!p.WriteBack(entity, in c))
                {
                    p.Add(entity, in c);
                }
            }
            else
            {
                AssertManaged<TComponent>();
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

            if (TryGetWritablePool<TComponent>(out var p))
            {
                p.WriteBack(entity, in data);
            }
            else
            {
                AssertManaged<TComponent>();
            }
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity)
        {
            return ReplaceComponent(entity, GetRegistration<TComponent>().Create());
        }

        public bool ReplaceComponent<TComponent>(TEntityKey entity, in TComponent c)
        {
            AssertValid(entity);

            if (TryGetWritablePool<TComponent>(out var p))
            {
                return p.WriteBack(entity, in c);
            }
            else
            {
                AssertManaged<TComponent>();
            }

            return false;
        }

        public void Sort<TComponent>(IComparer<TComponent> comparator)
        {
            if (TryGetPool<TComponent>(out var pool) &&
                pool is ISortableCollection<TComponent> sortablePool)
            {
                sortablePool.HeapSort(comparator);
            }
        }

        public void Respect<TComponentTo, TComponentFrom>()
        {
            if (TryGetWritablePool<TComponentTo>(out var pool) &&
                TryGetPool<TComponentFrom>(out var other))
            {
                pool.Respect(other);
            }
        }

        public void ResetComponent<TComponent>()
        {
            if (TryGetWritablePool<TComponent>(out var pool))
            {
                pool.RemoveAll();
            }
        }

        public void CopyTo(List<TEntityKey> k)
        {
            k.Clear();
            k.Capacity = Math.Max(k.Capacity, Count);
            foreach (var e in this)
            {
                k.Add(e);
            }
        }

        public void Clear()
        {
            var l = EntityKeyListPool<TEntityKey>.Reserve(this);
            foreach (var last in l)
            {
                if (IsValid(last))
                {
                    Destroy(last);
                }
                else
                {
                    logger.Verbose("Invalid entry {Entity}", last);
                }
            }

            EntityKeyListPool<TEntityKey>.Release(l);

            if (!IsEmpty)
            {
                // someone is doing something silly, like creating new entities
                // during create. Nuke the beast from orbit ..

                foreach (var pool in pools)
                {
                    if (pool.TryGetPool(out var p))
                    {
                        p.RemoveAll();
                    }
                }
            }

            entities.Clear();
            available = 0;
            next = default;
        }

        public void Reset(TEntityKey entity)
        {
            foreach (var pool in pools)
            {
                if (pool.TryGetPool(out var p))
                {
                    p.Remove(entity);
                }
            }
        }

        public bool IsOrphan(TEntityKey e)
        {
            AssertValid(e);
            var orphan = true;
            foreach (var pool in pools)
            {
                orphan &= !pool.ReadonlyPool.Contains(e);
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
            public readonly object? Tag;

            public Attachment(TEntityKey entity, object? tag)
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
                current = default!;
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

                current = default!;
                return false;
            }

            public void Reset()
            {
                index = -1;
                current = default!;
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