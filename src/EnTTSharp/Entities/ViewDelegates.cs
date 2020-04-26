namespace EnttSharp.Entities
{
  public static class ViewDelegates
  {
    public delegate void Apply(IEntityViewControl v, EntityKey k);

    public delegate void Apply<T1>(IEntityViewControl v, EntityKey k, in T1 c1);

    public delegate void Apply<T1, T2>(IEntityViewControl v, EntityKey k, in T1 c1, in T2 c2);

    public delegate void Apply<T1, T2, T3>(IEntityViewControl v, EntityKey k, in T1 c1, in T2 c2, in T3 c3);

    public delegate void Apply<T1, T2, T3, T4>(IEntityViewControl v, EntityKey k, in T1 c1, in T2 c2, in T3 c3, in T4 c4);

    public delegate void Apply<T1, T2, T3, T4, T5>(IEntityViewControl v, EntityKey k, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5);

    public delegate void Apply<T1, T2, T3, T4, T5, T6>(IEntityViewControl v, EntityKey k, in T1 c1, in T2 c2, in T3 c3, in T4 c4, in T5 c5, in T6 c6);

    public delegate void Apply<T1, T2, T3, T4, T5, T6, T7>(IEntityViewControl v,
                                                           EntityKey k,
                                                           in T1 c1,
                                                           in T2 c2,
                                                           in T3 c3,
                                                           in T4 c4,
                                                           in T5 c5,
                                                           in T6 c6,
                                                           in T7 c7);

    public delegate void ApplyWithContext<in TContext>(IEntityViewControl v, TContext context, EntityKey k);

    public delegate void ApplyWithContext<in TContext, T1>(IEntityViewControl v, TContext context, EntityKey k, in T1 c1);

    public delegate void ApplyWithContext<in TContext, T1, T2>(IEntityViewControl v, TContext context, EntityKey k, in T1 c1, in T2 c2);

    public delegate void ApplyWithContext<in TContext, T1, T2, T3>(IEntityViewControl v, TContext context, EntityKey k, in T1 c1, in T2 c2, in T3 c3);

    public delegate void
      ApplyWithContext<in TContext, T1, T2, T3, T4>(IEntityViewControl v, TContext context, EntityKey k, in T1 c1, in T2 c2, in T3 c3, in T4 c4);

    public delegate void ApplyWithContext<in TContext, T1, T2, T3, T4, T5>(IEntityViewControl v,
                                                                           TContext context,
                                                                           EntityKey k,
                                                                           in T1 c1,
                                                                           in T2 c2,
                                                                           in T3 c3,
                                                                           in T4 c4,
                                                                           in T5 c5);

    public delegate void ApplyWithContext<in TContext, T1, T2, T3, T4, T5, T6>(IEntityViewControl v,
                                                                               TContext context,
                                                                               EntityKey k,
                                                                               in T1 c1,
                                                                               in T2 c2,
                                                                               in T3 c3,
                                                                               in T4 c4,
                                                                               in T5 c5,
                                                                               in T6 c6);

    public delegate void ApplyWithContext<in TContext, T1, T2, T3, T4, T5, T6, T7>(IEntityViewControl v,
                                                                                   TContext context,
                                                                                   EntityKey k,
                                                                                   in T1 c1,
                                                                                   in T2 c2,
                                                                                   in T3 c3,
                                                                                   in T4 c4,
                                                                                   in T5 c5,
                                                                                   in T6 c6,
                                                                                   in T7 c7);
  }
}
