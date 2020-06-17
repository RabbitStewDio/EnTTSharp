namespace EnttSharp.Entities
{
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
