using System;

namespace EnTTSharp.Entities.Systems
{
    public readonly struct EntitySystemReference
    {
        public string SystemId { get; }
        public Action System { get; }

        public EntitySystemReference(string systemId, Action system)
        {
            SystemId = systemId;
            System = system;
        }

        public static implicit operator Action(EntitySystemReference r)
        {
            return r.System;
        }

        public static EntitySystemReference Create(Action action, Delegate source)
        {
            var systemId = CreateSystemDescription(source);
            return new EntitySystemReference(systemId, action);
        }

        public static string CreateSystemDescription(Delegate source)
        {
            var targetType = source.Target?.GetType() ?? source.Method.DeclaringType;
            var systemId = $"{NameWithoutGenerics(targetType)}#{source.Method.Name}";
            return systemId;
        }

        static string NameWithoutGenerics(Type t)
        {
            if (t == null) return "<??>";
            
            var name = t.FullName ?? t.Name;
            var index = name.IndexOf('`');
            return index == -1 ? name : name.Substring(0, index);
        }
        
        public static EntitySystemReference<TContext> Create<TContext>(Action<TContext> action, Delegate source)
        {
            var systemId = CreateSystemDescription(source);
            return new EntitySystemReference<TContext>(systemId, action);
        }
    }

    public readonly struct EntitySystemReference<TContext>
    {
        public string SystemId { get; }
        public Action<TContext> System { get; }

        public EntitySystemReference(string systemId, Action<TContext> system)
        {
            SystemId = systemId;
            System = system;
        }

        public static implicit operator Action<TContext>(EntitySystemReference<TContext> r)
        {
            return r.System;
        }

    }
}