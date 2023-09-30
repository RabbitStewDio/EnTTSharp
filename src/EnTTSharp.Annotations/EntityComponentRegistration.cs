using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        public bool IsEmpty => data.Count == 0;
        public TypeInfo TypeInfo { get; }

        public void Store<TData>(TData value)
        {
            object? o = value;
            if (o != null)
            {
                data[typeof(TData)] = o;
            }
        }

        public bool TryGet<TData>([MaybeNullWhen(false)] out TData result)
        {
            if (this.data.TryGetValue(typeof(TData), out var raw) &&
                raw is TData typed)
            {
                result = typed;
                return true;
            }

            result = default!;
            return false;
        }

        public override string ToString()
        {
            return $"EntityComponentRegistration({nameof(TypeInfo)}: {TypeInfo}, {nameof(IsEmpty)}: {IsEmpty})";
        }
    }
}