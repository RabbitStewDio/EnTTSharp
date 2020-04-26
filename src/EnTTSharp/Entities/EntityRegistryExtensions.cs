namespace EnttSharp.Entities
{
  public static class EntityRegistryExtensions
  {
    public static bool HasComponent<T1, T2>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e);
    }

    public static bool HasComponent<T1, T2, T3>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e);
    }

    public static bool HasComponent<T1, T2, T3, T4>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e);
    }

    public static bool HasComponent<T1, T2, T3, T4, T5>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e) && reg.HasComponent<T5>(e);
    }

    public static bool HasComponent<T1, T2, T3, T4, T5, T6>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e)
             && reg.HasComponent<T2>(e)
             && reg.HasComponent<T3>(e)
             && reg.HasComponent<T4>(e)
             && reg.HasComponent<T5>(e)
             && reg.HasComponent<T6>(e);
    }

    public static bool HasComponent<T1, T2, T3, T4, T5, T6, T7>(this IEntityViewControl reg, EntityKey e)
    {
      return reg.HasComponent<T1>(e)
             && reg.HasComponent<T2>(e)
             && reg.HasComponent<T3>(e)
             && reg.HasComponent<T4>(e)
             && reg.HasComponent<T5>(e)
             && reg.HasComponent<T6>(e)
             && reg.HasComponent<T7>(e);
    }

    public static bool GetComponent<T1, T2>(this IEntityViewControl reg, EntityKey entity, out T1 c1, out T2 c2)
    {
      if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      return false;
    }

    public static bool GetComponent<T1, T2, T3>(this IEntityViewControl reg,
                                                EntityKey entity,
                                                out T1 c1,
                                                out T2 c2,
                                                out T3 c3)
    {
      if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      c3 = default(T3);
      return false;
    }

    public static bool GetComponent<T1, T2, T3, T4>(this IEntityViewControl reg,
                                                    EntityKey entity,
                                                    out T1 c1,
                                                    out T2 c2,
                                                    out T3 c3,
                                                    out T4 c4)
    {
      if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3) && reg.GetComponent(entity, out c4))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      c3 = default(T3);
      c4 = default(T4);
      return false;
    }

    public static bool GetComponent<T1, T2, T3, T4, T5>(this IEntityViewControl reg,
                                                        EntityKey entity,
                                                        out T1 c1,
                                                        out T2 c2,
                                                        out T3 c3,
                                                        out T4 c4,
                                                        out T5 c5)
    {
      if (reg.GetComponent(entity, out c1)
          && reg.GetComponent(entity, out c2)
          && reg.GetComponent(entity, out c3)
          && reg.GetComponent(entity, out c4)
          && reg.GetComponent(entity, out c5))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      c3 = default(T3);
      c4 = default(T4);
      c5 = default(T5);
      return false;
    }

    public static bool GetComponent<T1, T2, T3, T4, T5, T6>(this IEntityViewControl reg,
                                                            EntityKey entity,
                                                            out T1 c1,
                                                            out T2 c2,
                                                            out T3 c3,
                                                            out T4 c4,
                                                            out T5 c5,
                                                            out T6 c6)
    {
      if (reg.GetComponent(entity, out c1)
          && reg.GetComponent(entity, out c2)
          && reg.GetComponent(entity, out c3)
          && reg.GetComponent(entity, out c4)
          && reg.GetComponent(entity, out c5)
          && reg.GetComponent(entity, out c6))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      c3 = default(T3);
      c4 = default(T4);
      c5 = default(T5);
      c6 = default(T6);
      return false;
    }

    public static bool GetComponent<T1, T2, T3, T4, T5, T6, T7>(this IEntityViewControl reg,
                                                                EntityKey entity,
                                                                out T1 c1,
                                                                out T2 c2,
                                                                out T3 c3,
                                                                out T4 c4,
                                                                out T5 c5,
                                                                out T6 c6,
                                                                out T7 c7)
    {
      if (reg.GetComponent(entity, out c1)
          && reg.GetComponent(entity, out c2)
          && reg.GetComponent(entity, out c3)
          && reg.GetComponent(entity, out c4)
          && reg.GetComponent(entity, out c5)
          && reg.GetComponent(entity, out c6)
          && reg.GetComponent(entity, out c7))
      {
        return true;
      }

      c1 = default(T1);
      c2 = default(T2);
      c3 = default(T3);
      c4 = default(T4);
      c5 = default(T5);
      c6 = default(T6);
      c7 = default(T7);
      return false;
    }

    public static TComponent GetOrCreateComponent<TComponent>(this IEntityViewControl reg, EntityKey entity)
    {
      if (!reg.GetComponent(entity, out TComponent c))
      {
        c = reg.AssignComponent<TComponent>(entity);
      }

      return c;
    }

    public static void GetOrCreateComponent<TComponent>(this IEntityViewControl reg, EntityKey entity, out TComponent c)
    {
      if (!reg.GetComponent(entity, out c))
      {
        c = reg.AssignComponent<TComponent>(entity);
      }
    }
  }
}
