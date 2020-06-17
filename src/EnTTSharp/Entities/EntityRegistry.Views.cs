﻿namespace EnttSharp.Entities
{
    public partial class EntityRegistry
    {
        public void DiscardView<TView>() where TView : IEntityView
        {
            views.Remove(typeof(TView));
        }
        
/*        
        public IEntityView<T1> View<T1>()
        {
            return new AdhocView<T1>(this);
        }

        public IEntityView<T1, T2> View<T1, T2>()
        {
            return new AdhocView<T1, T2>(this);
        }

        public IEntityView<T1, T2, T3> View<T1, T2, T3>()
        {
            return new AdhocView<T1, T2, T3>(this);
        }

        public IEntityView<T1, T2, T3, T4> View<T1, T2, T3, T4>()
        {
            return new AdhocView<T1, T2, T3, T4>(this);
        }

        public IEntityView<T1, T2, T3, T4, T5> View<T1, T2, T3, T4, T5>()
        {
            return new AdhocView<T1, T2, T3, T4, T5>(this);
        }

        public IEntityView<T1, T2, T3, T4, T5, T6> View<T1, T2, T3, T4, T5, T6>()
        {
            return new AdhocView<T1, T2, T3, T4, T5, T6>(this);
        }

        public IEntityView<T1, T2, T3, T4, T5, T6, T7> View<T1, T2, T3, T4, T5, T6, T7>()
        {
            return new AdhocView<T1, T2, T3, T4, T5, T6, T7>(this);
        }

        public IPersistentEntityView<T1> PersistentView<T1>()
        {
            var type = typeof(PersistentView<T1>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1>)view;
            }

            var v = new PersistentView<T1>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2> PersistentView<T1, T2>()
        {
            var type = typeof(PersistentView<T1, T2>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2>)view;
            }

            var v = new PersistentView<T1, T2>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2, T3> PersistentView<T1, T2, T3>()
        {
            var type = typeof(PersistentView<T1, T2, T3>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2, T3>)view;
            }

            var v = new PersistentView<T1, T2, T3>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2, T3, T4> PersistentView<T1, T2, T3, T4>()
        {
            var type = typeof(PersistentView<T1, T2, T3, T4>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2, T3, T4>)view;
            }

            var v = new PersistentView<T1, T2, T3, T4>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2, T3, T4, T5> PersistentView<T1, T2, T3, T4, T5>()
        {
            var type = typeof(PersistentView<T1, T2, T3, T4, T5>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2, T3, T4, T5>)view;
            }

            var v = new PersistentView<T1, T2, T3, T4, T5>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2, T3, T4, T5, T6> PersistentView<T1, T2, T3, T4, T5, T6>()
        {
            var type = typeof(PersistentView<T1, T2, T3, T4, T5, T6>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2, T3, T4, T5, T6>)view;
            }

            var v = new PersistentView<T1, T2, T3, T4, T5, T6>(this);
            views[type] = v;
            return v;
        }

        public IPersistentEntityView<T1, T2, T3, T4, T5, T6, T7> PersistentView<T1, T2, T3, T4, T5, T6, T7>()
        {
            var type = typeof(PersistentView<T1, T2, T3, T4, T5, T6, T7>);
            if (views.TryGetValue(type, out var view))
            {
                return (IPersistentEntityView<T1, T2, T3, T4, T5, T6, T7>)view;
            }

            var v = new PersistentView<T1, T2, T3, T4, T5, T6, T7>(this);
            views[type] = v;
            return v;
        }
*/        
    }
}