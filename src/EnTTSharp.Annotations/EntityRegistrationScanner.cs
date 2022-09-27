using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnTTSharp.Entities.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace EnTTSharp.Annotations
{
    public class EntityRegistrationScanner
    {
        readonly List<IEntityRegistrationHandler> handlers;

        public EntityRegistrationScanner(params IEntityRegistrationHandler[] handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            this.handlers = handlers.ToList();
        }

        public EntityRegistrationScanner With(IEntityRegistrationHandler handler)
        {
            Register(handler);
            return this;
        }

        public void Register(IEntityRegistrationHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.handlers.Add(handler);
        }

        public List<EntityComponentRegistration> RegisterEntitiesFromAllAssemblies()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var retval = new List<EntityComponentRegistration>();
            foreach (var assembly in assemblies)
            {
                retval.AddRange(RegisterComponentsFromAssembly(assembly));
            }

            return retval;
        }

        public List<EntityComponentRegistration> RegisterComponentsFromAssembly(Assembly a,
                                                                                List<EntityComponentRegistration>? retval = null)
        {
            retval = retval ?? new List<EntityComponentRegistration>();
            foreach (var typeInfo in a.DefinedTypes)
            {
                if (TryRegisterComponent(typeInfo, out var r))
                {
                    retval.Add(r);
                }
            }

            return retval;
        }

        public bool TryRegisterComponent<TData>([MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            var type = typeof(TData);
            var typeInfo = type.GetTypeInfo();
            return TryRegisterComponent(typeInfo, out result);
        }

        public bool TryRegisterKey<TData>([MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            var type = typeof(TData);
            var typeInfo = type.GetTypeInfo();
            return TryRegisterKey(typeInfo, out result);
        }

        public bool TryRegisterComponent(TypeInfo typeInfo, [MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            if (typeInfo.IsAbstract)
            {
                result = default;
                return false;
            }

            if (!typeInfo.IsDefined(typeof(EntityComponentAttribute)))
            {
                result = default;
                return false;
            }

            result = new EntityComponentRegistration(typeInfo);
            foreach (var h in handlers)
            {
                h.Process(result);
            }

            return !result.IsEmpty;
        }

        public bool TryRegisterKey(TypeInfo typeInfo, [MaybeNullWhen(false)] out EntityComponentRegistration result)
        {
            if (typeInfo.IsAbstract)
            {
                result = default;
                return false;
            }

            if (!typeInfo.IsDefined(typeof(EntityKeyAttribute)))
            {
                result = default;
                return false;
            }

            result = new EntityComponentRegistration(typeInfo);
            foreach (var h in handlers)
            {
                h.Process(result);
            }

            return !result.IsEmpty;
        }
    }
}