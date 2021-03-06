﻿namespace EnTTSharp.Entities.Helpers
{
    public interface ISortableCollection<out T>
    {
        int Count { get; }
        T this[int index] { get; }
        void Swap(int idxSrc, int idxTgt);
    }
}