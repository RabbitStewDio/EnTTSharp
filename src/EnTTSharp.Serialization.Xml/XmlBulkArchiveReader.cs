using System.Xml;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlBulkArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        readonly XmlArchiveReaderBackend<TEntityKey> readerConfiguration;

        public XmlBulkArchiveReader(XmlReadHandlerRegistry registry): this(new XmlArchiveReaderBackend<TEntityKey>(registry))
        {
        }

        public XmlBulkArchiveReader(XmlArchiveReaderBackend<TEntityKey> readerConfiguration)
        {
            this.readerConfiguration = readerConfiguration;
        }

        public void ReadAllFragment(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (!HandleRootElement(reader, loader))
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        public void ReadAll(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == "snapshot")
                    {
                        ReadAllFragment(reader, loader);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        bool HandleRootElement(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            switch (reader.Name)
            {
                case XmlTagNames.Entities:
                {
                    ReadEntities(reader, loader);
                    return true;
                }
                case XmlTagNames.DestroyedEntities:
                {
                    ReadDestroyed(reader, loader);
                    return true;
                }
                case XmlTagNames.Components:
                {
                    ReadComponent(reader, loader);
                    return true;
                }
                case XmlTagNames.Tag:
                {
                    readerConfiguration.ReadTag(reader, loader);
                    return true;
                }
            }
            return false;
        }


        void ReadEntities(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == XmlTagNames.Entity)
                    {
                        var entity = readerConfiguration.ReadEntity(reader, loader.Map);
                        loader.OnEntity(entity);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        void ReadDestroyed(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == XmlTagNames.DestroyedEntity)
                    {
                        var entity = readerConfiguration.ReadDestroyedEntity(reader, loader.Map);
                        loader.OnDestroyedEntity(entity);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }

        void ReadComponent(XmlReader reader, ISnapshotLoader<TEntityKey> loader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == XmlTagNames.Component)
                    {
                        readerConfiguration.ReadComponent(reader, loader);
                    }
                    else
                    {
                        reader.Skip();
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    return;
                }
            }
        }        

    }
}