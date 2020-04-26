using System;
using System.Collections.Generic;
using System.Reflection;

namespace EnTTSharp.Annotations
{
    public class EntityComponentRegistration
    {
        readonly Dictionary<Type, object> data;

        public EntityComponentRegistration(TypeInfo typeInfo)
        {
            this.TypeInfo = typeInfo;
            this.data = new Dictionary<Type, object>();
        }

        public TypeInfo TypeInfo { get; }

        public void Store<TData>(TData value)
        {
            data[typeof(TData)] = value;
        }

        public bool TryGet<TData>(out TData result)
        {
            if (this.data.TryGetValue(typeof(TData), out var raw) &&
                raw is TData typed)
            {
                result = typed;
                return true;
            }

            result = default;
            return false;
        }
    }
}