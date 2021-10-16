using System;
using System.Text.RegularExpressions;

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
            if (IsClosure(targetType, out var baseType))
            {
                var systemId = $"{NameWithoutGenerics(baseType)}#{ClosureMethodNameWithContext(baseType, source)}";
                return systemId;
            }
            else
            {
                var systemId = $"{NameWithoutGenerics(targetType)}#{source.Method.Name}";
                return systemId;
            }
        }

        static string ClosureMethodNameWithContext(Type t, Delegate action)
        {
            var rawName = action.Method.Name;
            var re = new Regex(@"<(?<Source>.*)>.__(?<Method>.*)\|.*");
            var m = re.Match(rawName);
            if (!m.Success)
            {
                return rawName;
            }
            
            var methodName = m.Groups["Method"]?.Value;
            var sourceMethod = m.Groups["Source"]?.Value;
            return $"{sourceMethod}.{methodName}";
        }

        static bool IsClosure(Type t, out Type baseType)
        {
            if (t == null)
            {
                baseType = default;
                return false;
            }
            
            while (t != null && t.Name.StartsWith("<"))
            {
                // closure class. Lets find the source class that declared this one.
                if (t.DeclaringType == null)
                {
                    break;
                }
                
                t = t.DeclaringType;
            }
            
            baseType = t;
            return true;
        }
        
        static string NameWithoutGenerics(Type t)
        {
            if (t == null) return "<??>";

            // var name = t.FullName ?? t.Name;
            var name = t.Name;
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