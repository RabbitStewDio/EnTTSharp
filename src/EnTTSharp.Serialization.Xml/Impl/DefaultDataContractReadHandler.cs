using System;
using System.Runtime.Serialization;
using System.Xml;
using EnttSharp.Entities;

namespace EnTTSharp.Serialization.Xml
{
    public class DefaultDataContractReadHandler<TComponent>
    {
        readonly DataContractSerializer serializer;
        readonly TranslateFunction<TComponent> translate;

        public DefaultDataContractReadHandler(TranslateFunction<TComponent> translate = null) : this(new DataContractSerializer(typeof(TComponent)), translate)
        {
        }

        public DefaultDataContractReadHandler(DataContractSerializer serializer,
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
            var t = (TComponent)serializer.ReadObject(reader);
            return translate(t, entityTranslator);
        }
    }
}