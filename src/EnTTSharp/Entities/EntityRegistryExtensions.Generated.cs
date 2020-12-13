


//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
namespace EnTTSharp.Entities
{
    public partial class EntityRegistryExtensions 
    {
        public static bool HasComponent<TEntityKey, T1, T2>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            return false;
        }

        public static bool HasComponent<TEntityKey, T1, T2, T3>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2, T3>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2, out T3 c3)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            c3 = default;
            return false;
        }

        public static bool HasComponent<TEntityKey, T1, T2, T3, T4>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2, T3, T4>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2, out T3 c3, out T4 c4)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3) && reg.GetComponent(entity, out c4))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            c3 = default;
            c4 = default;
            return false;
        }

        public static bool HasComponent<TEntityKey, T1, T2, T3, T4, T5>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e) && reg.HasComponent<T5>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2, T3, T4, T5>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3) && reg.GetComponent(entity, out c4) && reg.GetComponent(entity, out c5))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            c3 = default;
            c4 = default;
            c5 = default;
            return false;
        }

        public static bool HasComponent<TEntityKey, T1, T2, T3, T4, T5, T6>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e) && reg.HasComponent<T5>(e) && reg.HasComponent<T6>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2, T3, T4, T5, T6>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5, out T6 c6)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3) && reg.GetComponent(entity, out c4) && reg.GetComponent(entity, out c5) && reg.GetComponent(entity, out c6))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            c3 = default;
            c4 = default;
            c5 = default;
            c6 = default;
            return false;
        }

        public static bool HasComponent<TEntityKey, T1, T2, T3, T4, T5, T6, T7>(this IEntityViewControl<TEntityKey> reg, TEntityKey e)
            where TEntityKey : IEntityKey
        {
            return reg.HasComponent<T1>(e) && reg.HasComponent<T2>(e) && reg.HasComponent<T3>(e) && reg.HasComponent<T4>(e) && reg.HasComponent<T5>(e) && reg.HasComponent<T6>(e) && reg.HasComponent<T7>(e);
        }

        public static bool GetComponent<TEntityKey, T1, T2, T3, T4, T5, T6, T7>(this IEntityViewControl<TEntityKey> reg, TEntityKey entity, 
                                                             out T1 c1, out T2 c2, out T3 c3, out T4 c4, out T5 c5, out T6 c6, out T7 c7)
            where TEntityKey : IEntityKey
        {
            if (reg.GetComponent(entity, out c1) && reg.GetComponent(entity, out c2) && reg.GetComponent(entity, out c3) && reg.GetComponent(entity, out c4) && reg.GetComponent(entity, out c5) && reg.GetComponent(entity, out c6) && reg.GetComponent(entity, out c7))
            {
                return true;
            }

            c1 = default;
            c2 = default;
            c3 = default;
            c4 = default;
            c5 = default;
            c6 = default;
            c7 = default;
            return false;
        }



    }
}