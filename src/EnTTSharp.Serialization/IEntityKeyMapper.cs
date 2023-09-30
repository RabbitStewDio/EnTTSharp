namespace EnTTSharp.Serialization
{
    public interface IEntityKeyMapper
    {
        public TEntityKey EntityKeyMapper<TEntityKey>(EntityKeyData data);
    }
}