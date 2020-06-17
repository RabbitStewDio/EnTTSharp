using EnTTSharp.Annotations;

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