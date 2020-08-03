using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using EnTTSharp.Serialization.Xml;
using EnTTSharp.Serialization.Xml.Impl;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    [DataContract]
    public class DummyEnumObject
    {
        public static readonly DummyEnumObject OptionA = new DummyEnumObject("A");
        public static readonly DummyEnumObject OptionB = new DummyEnumObject("B");

        DummyEnumObject(string id)
        {
            this.Id = id;
        }

        [DataMember] public string Id { get; }
    }

    public class DummyEnumObjectSurrogateProvider : SerializationSurrogateProviderBase<DummyEnumObject, SurrogateContainer<string>>
    {
        public override DummyEnumObject GetDeserializedObject(SurrogateContainer<string> surrogate)
        {
            if (surrogate.Content == "A") return DummyEnumObject.OptionA;
            if (surrogate.Content == "B") return DummyEnumObject.OptionB;
            throw new ArgumentException();
        }

        public override SurrogateContainer<string> GetObjectToSerialize(DummyEnumObject obj)
        {
            return new SurrogateContainer<string>(obj.Id);
        }
    }

    [DataContract]
    public readonly struct DummyEnumDataContainer
    {
        [DataMember] public readonly DummyEnumObject da;

        public DummyEnumDataContainer(DummyEnumObject da)
        {
            this.da = da;
        }
    }

    public class DummyEnumResolverTest
    {
        [Test]
        public void TestDataContractResolving()
        {
            var model = new DummyEnumDataContainer(DummyEnumObject.OptionA);
            var surrogate = new ObjectSurrogateResolver();
            surrogate.Register(typeof(DummyEnumObject), new DummyEnumObjectSurrogateProvider());

            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  "
                                             });


            var dc = new DefaultDataContractWriteHandler<DummyEnumDataContainer>(surrogate);
            dc.Write(xmlWriter, model);
            xmlWriter.Flush();

            Console.WriteLine("----------------------");
            Console.WriteLine(sb.ToString());
            Console.WriteLine("----------------------");

            var xmlReader = XmlReader.Create(new StringReader(sb.ToString()));
            var dr = new DefaultDataContractReadHandler<DummyEnumDataContainer>(surrogate);
            var loaded = dr.Read(xmlReader);
            loaded.da.Should().BeSameAs(DummyEnumObject.OptionA);
        }
    }
}