namespace EnTTSharp.Entities
{
    public interface IComponentRegistration
    {
        int Index { get; }
    }

    public interface IComponentRegistration<in TEntityKey, T> : IComponentRegistration
        where TEntityKey: IEntityKey
    {
        T Create();
        void Destruct(TEntityKey k, T o);
        bool HasDestructor();
    }
}