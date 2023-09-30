using System;
using System.IO;
using System.Text;
using System.Xml;
using EnTTSharp.Serialization.Xml;
using EnTTSharp.Serialization.Xml.Impl;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation.Surrogates
{
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