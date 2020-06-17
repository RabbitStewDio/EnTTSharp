using System;
using EnttSharp.Entities.Helpers;

namespace EnttSharp.Entities
{
    public abstract class PersistentMultiViewBase : MultiViewBase<PredicateEnumerator<EntityKey>>
    {
        readonly SparseSet view;
        protected readonly Func<EntityKey, bool> FastIsMemberPredicate;

        protected PersistentMultiViewBase(EntityRegistry registry,
                                          params ISparsePool[] entries) : base(registry, entries)
        {
            view = SparseSet.CreateFrom(CreateInitialEnumerator(entries));
            FastIsMemberPredicate = view.Contains;
        }

        protected override int EstimatedSize => view.Count;

        protected override void OnDestroyed(object sender, EntityKey e)
        {
            base.OnDestroyed(sender, e);
            view.Remove(e);
        }

        protected override void OnCreated(object sender, EntityKey e)
        {
            if (IsMember(e) && !view.Contains(e))
            {
                view.Add(e);
            }

            base.OnCreated(sender, e);
        }

        public int Count => EstimatedSize;

        public override bool Contains(EntityKey e)
        {
            return view.Contains(e);
        }

        public override PredicateEnumerator<EntityKey> GetEnumerator()
        {
            return new PredicateEnumerator<EntityKey>(view, FastIsMemberPredicate);
        }

        public override void Respect<TComponent>()
        {
            view.Respect(Registry.GetPool<TComponent>());
        }

        PredicateEnumerator<EntityKey> CreateInitialEnumerator(ISparsePool[] sets)
        {
            ISparsePool s = null;
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

            return new PredicateEnumerator<EntityKey>(s, IsMemberPredicate);
        }
    }
}