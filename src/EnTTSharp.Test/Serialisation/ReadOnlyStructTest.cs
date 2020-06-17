using FluentAssertions;
using MessagePack;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    public class ReadOnlyStructTest
    {
        [MessagePackObject(true)]
        public readonly struct Model
        {
            [Key(0)]
            public readonly int Number;
            [Key(1)]
            public readonly string Text;

            public Model(int number, string text)
            {
                Number = number;
                Text = text;
            }
        }

        [Test]
        public void Test()
        {
            var model = new Model(10, "Test");
            var data = MessagePackSerializer.Serialize(model);

            // Deserialize the model
            var loaded = MessagePackSerializer.Deserialize<Model>(data);
            loaded.Number.Should().Be(10);
            loaded.Text.Should().Be("Test");
        }
    }
}