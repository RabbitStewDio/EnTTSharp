using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using EnTTSharp.Serialization.Xml.Impl;
using FluentAssertions;
using MessagePack;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    public class ReadOnlyStructTest
    {
        [MessagePackObject]
        [DataContract]
        public readonly struct Model
        {
            [Key(0)]
            [DataMember]
            public readonly int Number;
            [Key(1)]
            [DataMember]
            public readonly string Text;

            public Model(int number, string text)
            {
                Number = number;
                Text = text;
            }
        }

        [Test]
        public void TestMessagePack()
        {
            var model = new Model(10, "Test");
            var data = MessagePackSerializer.Serialize(model);
            data.Length.Should().Be(7);
            // Deserialize the model
            var loaded = MessagePackSerializer.Deserialize<Model>(data);
            loaded.Number.Should().Be(10);
            loaded.Text.Should().Be("Test");
        }

        [Test]
        public void TestDataContractXml()
        {
            var model = new Model(10, "Test");

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  "
                                             });


            var dc = new DefaultDataContractWriteHandler<Model>(null);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Console.WriteLine("----------------------");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("----------------------");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<Model>(null);
            var loaded = dr.Read(xmlReader);
            loaded.Number.Should().Be(10);
            loaded.Text.Should().Be("Test");
        }
    }
}