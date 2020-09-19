using System.Diagnostics.CodeAnalysis;
using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Fixtures
{
    [EntityComponent()]
    public class NonDefaultConstrucableClassComponent
    {
        [SuppressMessage("ReSharper", "UnusedParameter.Local")]
        public NonDefaultConstrucableClassComponent(int dummy)
        {
        }
    }
}