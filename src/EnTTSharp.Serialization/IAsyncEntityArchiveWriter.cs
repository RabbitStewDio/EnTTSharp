using System.Threading.Tasks;

namespace EnTTSharp.Serialization
{
    public interface IAsyncEntityArchiveWriter<TEntityKey>
    {
        Task WriteStartEntityAsync(in int entityCount);
        Task WriteEntityAsync(in TEntityKey entityKey);
        Task WriteEndEntityAsync();

        Task WriteStartComponentAsync<TComponent>(in int entityCount);
        Task WriteComponentAsync<TComponent>(in TEntityKey entityKey, in TComponent c);
        Task WriteEndComponentAsync<TComponent>();

        Task WriteTagAsync<TComponent>(in TEntityKey entityKey, in TComponent c);
        Task WriteMissingTagAsync<TComponent>();

        Task WriteStartDestroyedAsync(in int entityCount);
        Task WriteDestroyedAsync(in TEntityKey entityKey);
        Task WriteEndDestroyedAsync();

        Task FlushFrameAsync();
    }
}