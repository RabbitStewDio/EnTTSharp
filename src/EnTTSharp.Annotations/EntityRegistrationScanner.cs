using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                retval.AddRange(RegisterEntitiesFromAssembly(assembly));
            }

            return retval;
        }

        public List<EntityComponentRegistration> RegisterEntitiesFromAssembly(Assembly a,
                                                                              List<EntityComponentRegistration> retval = null)
        {
            retval = retval ?? new List<EntityComponentRegistration>();
            foreach (var typeInfo in a.DefinedTypes)
            {
                if (TryRegisterEntity(typeInfo, out var r))
                {
                    retval.Add(r);
                }
            }

            return retval;
        }

        public bool TryRegisterEntity(TypeInfo typeInfo, out EntityComponentRegistration result)
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

            return true;
        }
    }
}