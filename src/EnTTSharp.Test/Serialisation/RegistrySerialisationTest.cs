﻿using System;
using System.IO;
using System.Text;
using System.Xml;
using EnttSharp.Entities;
using EnTTSharp.Serialization;
using EnTTSharp.Serialization.Xml;
using EnTTSharp.Test.Fixtures;
using FluentAssertions;
using NUnit.Framework;

namespace EnTTSharp.Test.Serialisation
{
    public class RegistrySerialisationTest
    {
        EntityRegistry ereg;
        XmlWriteHandlerRegistry wreg;
        XmlReadHandlerRegistry rreg;

        [SetUp]
        public void SetUp()
        {
            wreg = new XmlWriteHandlerRegistry();
            wreg.Register(XmlWriteHandlerRegistration.Create<TestStructFixture>(new DefaultDataContractWriteHandler<TestStructFixture>().Write, false));
            wreg.Register(XmlWriteHandlerRegistration.Create<StringBuilder>(new DefaultWriteHandler<StringBuilder>().Write, false));
            wreg.Register(XmlWriteHandlerRegistration.Create<int>(new DefaultWriteHandler<int>().Write, true));
            wreg.Register(XmlWriteHandlerRegistration.Create<float>(new DefaultWriteHandler<float>().Write, true));

            rreg = new XmlReadHandlerRegistry();
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultDataContractReadHandler<TestStructFixture>().Read, false));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<StringBuilder>().Read, false));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<int>().Read, true));
            rreg.Register(XmlReadHandlerRegistration.Create(new DefaultReadHandler<float>().Read, true));

            ereg = new EntityRegistry();
            ereg.Register<TestStructFixture>();
            ereg.Register<StringBuilder>();
            ereg.Register<int>();
            ereg.Register<float>();
            ereg.CreateAsActor().AssignComponent(new TestStructFixture());
            ereg.CreateAsActor().AssignComponent<StringBuilder>().AttachTag(100);
        }

        [Test]
        public void TestPersistentView()
        {
            var sb = WriteRegistry(ereg, wreg, true);

            Console.WriteLine(sb);

            var nreg = new EntityRegistry();
            nreg.Register<TestStructFixture>();
            nreg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(sb));
            xmlReader.AdvanceToElement("snapshot");

            var readerConfig = new XmlBulkArchiveReader(rreg);
            readerConfig.ReadAll(xmlReader, nreg.CreateLoader());

            var result = WriteRegistry(nreg, wreg, true);
            Console.WriteLine(result);
            result.Should().Be(sb);
        }

        [Test]
        public void TestStreamingView()
        {
            var sb = WriteRegistry(ereg, wreg, false);

            Console.WriteLine(sb);

            var nreg = new EntityRegistry();
            nreg.Register<TestStructFixture>();
            nreg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(sb));
            xmlReader.AdvanceToElement("snapshot");

            var streamReader = new SnapshotStreamReader(nreg.CreateLoader());
            var archiveReader = new XmlEntityArchiveReader(rreg, xmlReader);
            streamReader.ReadDestroyed(archiveReader)
                        .ReadEntities(archiveReader)
                        .ReadComponent<TestStructFixture>(archiveReader)
                        .ReadComponent<StringBuilder>(archiveReader)
                        .ReadTag<int>(archiveReader);

            var result = WriteRegistry(nreg, wreg, false);
            Console.WriteLine(result);
            result.Should().Be(sb);
        }

        [Test]
        public void TestAutomaticStreamingView()
        {
            var sb = WriteRegistry(ereg, wreg, true);

            Console.WriteLine(sb);

            var nreg = new EntityRegistry();
            nreg.Register<TestStructFixture>();
            nreg.Register<StringBuilder>();

            var xmlReader = XmlReader.Create(new StringReader(sb));
            xmlReader.AdvanceToElement("snapshot");

            var streamReader = new SnapshotStreamReader(nreg.CreateLoader());
            var archiveReader = new XmlEntityArchiveReader(rreg, xmlReader);
            streamReader.ReadAll(archiveReader);

            var result = WriteRegistry(nreg, wreg, true);
            Console.WriteLine(result);
            result.Should().Be(sb);
        }

        static string WriteRegistry(EntityRegistry ereg,
                                    XmlWriteHandlerRegistry writerRegistry,
                                    bool automaticHandlers)
        {
            var sb = new StringBuilder();
            var xmlWriter = XmlWriter.Create(sb,
                                             new XmlWriterSettings()
                                             {
                                                 Indent = true,
                                                 IndentChars = "  "
                                             });
            var output = new XmlArchiveWriter(writerRegistry, xmlWriter);
            output.WriteDefaultSnapshotDocumentHeader();
            var snapshotView = ereg.CreateSnapshot();
            if (automaticHandlers)
            {
                snapshotView.WriteAll(output);
            }
            else
            {
                snapshotView
                    .WriteDestroyed(output)
                    .WriteEntites(output)
                    .WriteComponent<TestStructFixture>(output)
                    .WriteComponent<StringBuilder>(output)
                    .WriteTag<int>(output)
                    .WriteTag<float>(output);
            }

            output.WriteDefaultSnapshotDocumentFooter();
            xmlWriter.Flush();
            return sb.ToString();
        }
    }
}