namespace EnttSharp.Entities
{
    public static partial class ViewDelegates
    {
        public delegate void Apply(IEntityViewControl v, EntityKey k);
        public delegate void ApplyWithContext<in TContext>(IEntityViewControl v, TContext context, EntityKey k);
    }
}
