using EnTTSharp.Annotations;
using EnTTSharp.Entities.Attributes;

namespace EnTTSharp.Test.Annotations
{
    [EntityComponent()]
    public class NonDefaultConstrucableClassComponent
    {
        public NonDefaultConstrucableClassComponent(int dummy)
        {
        }
    }
}