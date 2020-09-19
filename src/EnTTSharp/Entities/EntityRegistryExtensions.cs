namespace EnTTSharp.Entities
{
    public static partial class EntityRegistryExtensions
    {
        public static TComponent GetOrCreateComponent<TEntityKey, TComponent>(this IEntityViewControl<TEntityKey> reg, 
                                                                              TEntityKey entity)
            where TEntityKey : IEntityKey
        {
            if (!reg.GetComponent(entity, out TComponent c))
            {
                c = reg.AssignComponent<TComponent>(entity);
            }

            return c;
        }

        public static void GetOrCreateComponent<TEntityKey, TComponent>(this IEntityViewControl<TEntityKey> reg, 
                                                                        TEntityKey entity, out TComponent c) 
            where TEntityKey : IEntityKey
        {
            if (!reg.GetComponent(entity, out c))
            {
                c = reg.AssignComponent<TComponent>(entity);
            }
        }
    }
}