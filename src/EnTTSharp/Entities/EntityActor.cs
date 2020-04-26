using System;

namespace EnttSharp.Entities
{
  public struct EntityActor
  {
    readonly EntityKey entity;
    readonly IEntityViewControl registry;

    public EntityActor(EntityRegistry registry)
    {
      this.registry = registry ?? throw new ArgumentNullException(nameof(registry));
      this.entity = registry.Create();
    }

    public EntityActor(IEntityViewControl registry, EntityKey entity)
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
      if (registry.TryGetTag(out EntityKey k, out TTag _))
      {
        return k == entity;
      }
      return false;
    }

    public EntityActor AttachTag<TTag>()
    {
      registry.AttachTag<TTag>(entity);
      return this;
    }

    public EntityActor AttachTag<TTag>(TTag tag)
    {
      registry.AttachTag(entity, tag);
      return this;
    }

    public EntityActor RemoveTag<TTag>()
    {
      registry.RemoveTag<TTag>();
      return this;
    }

    public bool TryGetTag<TTag>(out TTag tag)
    {
      if (registry.TryGetTag(out EntityKey other, out tag))
      {
        if (other == entity)
        {
          return true;
        }
      }

      tag = default(TTag);
      return false;
    }

    public EntityKey Entity
    {
      get { return entity; }
    }

    public EntityActor AssignComponent<TComponent>()
    {
      registry.AssignComponent<TComponent>(entity);
      return this;
    }

    public EntityActor AssignComponent<TComponent>(TComponent c)
    {
      registry.AssignComponent(entity, c);
      return this;
    }

    public EntityActor AssignComponent<TComponent>(in TComponent c)
    {
      registry.AssignComponent(entity, in c);
      return this;
    }

    public EntityActor RemoveComponent<TComponent>()
    {
      registry.RemoveComponent<TComponent>(entity);
      return this;
    }

    public bool HasComponent<TComponent>()
    {
      return registry.HasComponent<TComponent>(entity);
    }

    public bool GetComponent<TComponent>(out TComponent c)
    {
      return registry.GetComponent(entity, out c);
    }

    public EntityActor AssignOrReplace<TComponent>(in TComponent c)
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

    public void WriteBack<TComponent>(in TComponent c) where TComponent: struct
    {
      registry.WriteBack(entity, in c);
    }
  }

  public static class EntityActorExtensions
  {
    public static EntityActor CreateAsActor(this EntityRegistry reg)
    {
      return new EntityActor(reg);
    }

    public static EntityActor AsActor(this EntityRegistry reg, EntityKey k)
    {
      return new EntityActor(reg, k);
    }


  }
}
