using System;
using System.Xml;
using System.Xml.Serialization;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public delegate TComponent TranslateFunction<TComponent>(in TComponent original, Func<EntityKey, EntityKey> keyMapper);

    public class DefaultReadHandler<TComponent>
    {

        readonly XmlSerializer serializer;
        readonly TranslateFunction<TComponent> translate;

        public DefaultReadHandler(TranslateFunction<TComponent> translate = null) : this(new XmlSerializer(typeof(TComponent)), translate)
        {
        }

        public DefaultReadHandler(XmlSerializer serializer,
                                  TranslateFunction<TComponent> translate = null)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.translate = translate ?? TranslateNoOp;
        }

        static TComponent TranslateNoOp(in TComponent arg1, Func<EntityKey, EntityKey> arg2)
        {
            return arg1;
        }

        public TComponent Read(XmlReader reader, Func<EntityKey, EntityKey> entityTranslator)
        {
            var t = (TComponent)serializer.Deserialize(reader);
            return translate(t, entityTranslator);
        }
    }
}