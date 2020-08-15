namespace EnTTSharp.Entities
{
    public static class EntityActorExtensions
    {
        public static EntityActor<TEntityKey> CreateAsActor<TEntityKey>(this EntityRegistry<TEntityKey> reg) 
            where TEntityKey : IEntityKey
        {
            return EntityActor.Create(reg);
        }

        public static EntityActor<TEntityKey> AsActor<TEntityKey>(this EntityRegistry<TEntityKey> reg, TEntityKey k) 
            where TEntityKey : IEntityKey
        {
            return new EntityActor<TEntityKey>(reg, k);
        }
    }
}
