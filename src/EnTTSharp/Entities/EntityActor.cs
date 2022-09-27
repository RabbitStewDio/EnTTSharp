using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Entities
{
    public static class EntityActor
    {
        public static EntityActor<TEntityKey> Create<TEntityKey>
            (EntityRegistry<TEntityKey> registry) where TEntityKey : IEntityKey
        {
            var entity = registry.Create();
            return new EntityActor<TEntityKey>(registry, entity);
        }
    }

    public readonly struct EntityActor<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly EqualityComparer<TEntityKey> EqualityHandler = EqualityComparer<TEntityKey>.Default;
        readonly TEntityKey entity;
        readonly IEntityViewControl<TEntityKey> registry;

        internal EntityActor(IEntityViewControl<TEntityKey> registry, TEntityKey entity)
        {
            this.entity = entity;
            this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public bool IsValid()
        {
            return registry.Contains(entity);
        }

        public bool HasTag<TTag>()
        {
            if (registry.TryGetTag<TTag>(out var k, out _))
            {
                return EqualityHandler.Equals(k, entity);
            }

            return false;
        }

        public EntityActor<TEntityKey> AttachTag<TTag>()
        {
            registry.AttachTag<TTag>(entity);
            return this;
        }

        public EntityActor<TEntityKey> AttachTag<TTag>(TTag tag)
        {
            registry.AttachTag(entity, tag);
            return this;
        }

        public EntityActor<TEntityKey> RemoveTag<TTag>()
        {
            registry.RemoveTag<TTag>();
            return this;
        }

        public bool TryGetTag<TTag>(out Optional<TTag> tag)
        {
            if (registry.TryGetTag(out var other, out tag))
            {
                if (EqualityHandler.Equals(other, entity))
                {
                    return true;
                }
            }

            tag = default;
            return false;
        }

        public TEntityKey Entity
        {
            get { return entity; }
        }

        public EntityActor<TEntityKey> AssignComponent<TComponent>()
        {
            registry.AssignComponent<TComponent>(entity);
            return this;
        }

        public EntityActor<TEntityKey> AssignComponent<TComponent>(TComponent c)
        {
            registry.AssignComponent(entity, c);
            return this;
        }

        public EntityActor<TEntityKey> AssignComponent<TComponent>(in TComponent c)
        {
            registry.AssignComponent(entity, in c);
            return this;
        }

        public EntityActor<TEntityKey> RemoveComponent<TComponent>()
        {
            registry.RemoveComponent<TComponent>(entity);
            return this;
        }

        public bool HasComponent<TComponent>()
        {
            return registry.HasComponent<TComponent>(entity);
        }

        public bool GetComponent<TComponent>([MaybeNullWhen(false)] out TComponent c)
        {
            return registry.GetComponent(entity, out c);
        }

        public EntityActor<TEntityKey> AssignOrReplace<TComponent>(in TComponent c)
        {
            registry.AssignOrReplace(entity, in c);
            return this;
        }

        public bool ReplaceComponent<TComponent>(in TComponent c)
        {
            return registry.ReplaceComponent(entity, in c);
        }

        public bool IsOrphan()
        {
            return registry.IsOrphan(entity);
        }

        public void WriteBack<TComponent>(in TComponent c) where TComponent : struct
        {
            registry.WriteBack(entity, in c);
        }

        public static implicit operator TEntityKey(EntityActor<TEntityKey> self) => self.Entity;
    }
}