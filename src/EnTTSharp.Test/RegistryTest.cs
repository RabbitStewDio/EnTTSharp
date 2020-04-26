using System;
using System.IO;
using System.Text;
using System.Xml;
using EnttSharp.Entities;
using EnTTSharp.Serialization;
using EnTTSharp.Serialization.Xml;
using NUnit.Framework;

namespace EnTTSharp.Test
{
    public class RegistryTest
    {
        [Test]
        public void TestPersistentView()
        {
            var wreg = new XmlWriteHandlerRegistry();
            wreg.Register(XmlWriteHandlerRegistration.Create<TestStructFixture>(new DefaultDataContractWriteHandler<TestStructFixture>().Write));
            wreg.Register(XmlWriteHandlerRegistration.Create<StringBuilder>(new DefaultWriteHandler<StringBuilder>().Write));
            wreg.Register(XmlWriteHandlerRegistration.Create<int>(new DefaultWriteHandler<int>().Write));

            var rreg = new XmlReadHandlerRegistry();
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultDataContractReadHandler<TestStructFixture>().Read));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<StringBuilder>().Read));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<int>().Read));

            var ereg = new EntityRegistry();
            ereg.Register<TestStructFixture>();
            ereg.Register<StringBuilder>();
            ereg.Register<int>();
            ereg.CreateAsActor().AssignComponent(new TestStructFixture());
            ereg.CreateAsActor().AssignComponent<StringBuilder>();

            var sb = WriteRegistry(ereg, wreg);

            Console.WriteLine(sb);

            var nreg = new EntityRegistry();
            nreg.Register<TestStructFixture>();
            nreg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(sb));
            xmlReader.AdvanceToElement("snapshot");
            new XmlArchiveReader(rreg)
                .Read(xmlReader, nreg.CreateLoader());

            Console.WriteLine(WriteRegistry(nreg, wreg));
        }

        static string WriteRegistry(EntityRegistry ereg, XmlWriteHandlerRegistry writerRegistry)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  "
                                             });
            var output = new XmlArchiveWriter(writerRegistry, xmlWriter);
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("snapshot");
            xmlWriter.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            xmlWriter.WriteAttributeString("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");

            ereg.CreateSnapshot()
                .WriteDestroyed(output)
                .WriteEntites(output)
                .WriteComponent<TestStructFixture>(output)
                .WriteComponent<StringBuilder>(output)
                .WriteTag<int>(output);

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Flush();
            return sb.ToString();
        }
    }
}