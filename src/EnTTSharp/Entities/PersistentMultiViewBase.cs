using System;
using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{
    public abstract class PersistentMultiViewBase<TEntityKey> : MultiViewBase<TEntityKey, PredicateEnumerator<TEntityKey>> 
        where TEntityKey : IEntityKey
    {
        readonly SparseSet<TEntityKey> view;
        protected readonly Func<TEntityKey, bool> FastIsMemberPredicate;

        protected PersistentMultiViewBase(IEntityPoolAccess<TEntityKey> registry,
                                          params ISparsePool<TEntityKey>[] entries) : base(registry, entries)
        {
            view = SparseSet<TEntityKey>.CreateFrom(CreateInitialEnumerator(entries));
            FastIsMemberPredicate = view.Contains;
        }

        protected override int EstimatedSize => view.Count;

        protected override void OnDestroyed(object sender, TEntityKey e)
        {
            base.OnDestroyed(sender, e);
            view.Remove(e);
        }

        protected override void OnCreated(object sender, TEntityKey e)
        {
            if (IsMember(e) && !view.Contains(e))
            {
                view.Add(e);
            }

            base.OnCreated(sender, e);
        }

        public int Count => EstimatedSize;

        public override bool Contains(TEntityKey e)
        {
            return view.Contains(e);
        }

        public override PredicateEnumerator<TEntityKey> GetEnumerator()
        {
            return new PredicateEnumerator<TEntityKey>(view, FastIsMemberPredicate);
        }

        public override void Respect<TComponent>()
        {
            view.Respect(Registry.GetPool<TComponent>());
        }

        PredicateEnumerator<TEntityKey> CreateInitialEnumerator(ISparsePool<TEntityKey>[] sets)
        {
            ISparsePool<TEntityKey> s = null;
            var count = int.MaxValue;
            foreach (var set in sets)
            {
                if (set.Count < count)
                {
                    s = set;
                    count = s.Count;
                }
            }

            if (s == null)
            {
                throw new ArgumentException();
            }

            return new PredicateEnumerator<TEntityKey>(s, IsMemberPredicate);
        }
    }
}