using System.Xml;
using EnTTSharp.Entities;
using EnTTSharp.Serialization.Xml.Impl;
using Serilog;

namespace EnTTSharp.Serialization.Xml
{
    public class XmlBulkArchiveReader<TEntityKey> where TEntityKey : IEntityKey
    {
        static readonly ILogger Logger = LogHelper.ForContext<XmlBulkArchiveReader<TEntityKey>>();
        readonly XmlArchiveReaderBackend<TEntityKey> readerConfiguration;

        public XmlBulkArchiveReader(XmlReadHandlerRegistry registry) : this(new XmlArchiveReaderBackend<TEntityKey>(registry))
        {
        }

        public XmlBulkArchiveReader(XmlArchiveReaderBackend<TEntityKey> readerConfiguration)
        {
            this.readerConfiguration = readerConfiguration;
        }

        public void ReadAllFragment(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            Logger.Verbose("Begin ReadAllFragment");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (!HandleRootElement(reader, loader, mapper))
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
            finally
            {
                Logger.Verbose("End ReadAllFragment");
            }
        }

        public void ReadAll(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
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
                        ReadAllFragment(reader, loader, mapper);
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

        bool HandleRootElement(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            switch (reader.Name)
            {
                case XmlTagNames.Entities:
                {
                    ReadEntities(reader, loader, mapper);
                    return true;
                }
                case XmlTagNames.DestroyedEntities:
                {
                    ReadDestroyed(reader, loader, mapper);
                    return true;
                }
                case XmlTagNames.Components:
                {
                    var attr = reader.GetAttribute("type");
                    if (string.IsNullOrEmpty(attr))
                    {
                        throw reader.FromMissingAttribute(XmlTagNames.Component, "type");
                    }

                    ReadComponent(reader, loader, attr);
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


        void ReadEntities(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            Logger.Verbose("Begin ReadEntities");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.Entity)
                        {
                            var entity = readerConfiguration.ReadEntity(reader, mapper);
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
            finally
            {
                Logger.Verbose("End ReadEntities");
            }
        }

        void ReadDestroyed(XmlReader reader, ISnapshotLoader<TEntityKey> loader, IEntityKeyMapper mapper)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            Logger.Verbose("Begin ReadDestroyed");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.DestroyedEntity)
                        {
                            var entity = readerConfiguration.ReadDestroyedEntity(reader, mapper);
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
            finally
            {
                Logger.Verbose("End ReadDestroyed");
            }
        }

        void ReadComponent(XmlReader reader, ISnapshotLoader<TEntityKey> loader, string type)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            Logger.Verbose("Begin ReadComponent");
            try
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (reader.Name == XmlTagNames.Component)
                        {
                            readerConfiguration.ReadComponent(reader, loader, type);
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
            finally
            {
                Logger.Verbose("End ReadComponent");
            }
        }
    }
}