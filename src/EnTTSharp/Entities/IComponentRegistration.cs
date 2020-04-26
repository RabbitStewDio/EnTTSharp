namespace EnttSharp.Entities
{
    public interface IComponentRegistration
    {
        int Index { get; }
    }

    public interface IComponentRegistration<T> : IComponentRegistration
    {
        T Create();
        void Destruct(EntityKey k, T o);
        bool HasDestructor();
    }
}