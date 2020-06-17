namespace EnttSharp.Entities
{
    public static partial class EntityRegistryExtensions
    {
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