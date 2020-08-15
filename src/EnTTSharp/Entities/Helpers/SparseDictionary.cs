using System;

namespace EnTTSharp.Entities.Helpers
{
    public class SparseDictionary<TEntityKey, TComponent> : SparseSet<TEntityKey>, ISortableCollection<TComponent>
        where TEntityKey: IEntityKey
    {
        readonly RawList<TComponent> instances;

        public SparseDictionary()
        {
            instances = new RawList<TComponent>();
        }

        public override int Capacity
        {
            get { return base.Capacity; }
            set
            {
                base.Capacity = value;
                instances.Capacity = value;
            }
        }

        // public IEnumerable<TComponent> Instances => instances;

        public virtual void Add(TEntityKey e, in TComponent component)
        {
            base.Add(e);
            instances.Add(component);
        }

        public override bool Remove(TEntityKey e)
        {
            var idx = IndexOf(e);
            if (idx == -1)
            {
                return false;
            }

            instances[idx] = instances[instances.Count - 1];
            instances.RemoveLast();
            base.Remove(e);
            return true;
        }

        public TComponent this[int index]
        {
            get { return instances[index]; }
        }

        void ISortableCollection<TComponent>.Swap(int idxSrc, int idxTgt)
        {
            Swap(idxSrc, idxTgt);
        }

        protected override void Swap(int idxSrc, int idxTarget)
        {
            base.Swap(idxSrc, idxTarget);
            instances.Swap(idxSrc, idxTarget);
        }

        public override void RemoveAll()
        {
            base.RemoveAll();
            instances.Clear();
        }

        public bool TryGet(TEntityKey entity, out TComponent component)
        {
            var idx = IndexOf(entity);
            if (idx >= 0)
            {
                component = instances[idx];
                return true;
            }

            component = default;
            return false;
        }

        [Obsolete("Use WriteBack instead")]
        public virtual bool Replace(TEntityKey entity, in TComponent component)
        {
            return WriteBack(entity, in component);
        }

        public virtual bool WriteBack(TEntityKey entity, in TComponent component)
        {
            var idx = IndexOf(entity);
            if (idx == -1)
            {
                return false;
            }

            instances[idx] = component;
            return true;
        }
    }
}