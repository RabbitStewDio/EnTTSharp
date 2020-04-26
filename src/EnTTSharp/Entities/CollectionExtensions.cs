using System;
using System.Collections.Generic;

namespace EnttSharp.Entities
{
  public static class CollectionExtensions
  {
    public static void StoreAt<T>(this List<T> l, int index, T data)
    {
      if (l.Count <= index)
      {
        l.Capacity = Math.Max(l.Capacity, index + 1);
        while (l.Count <= index)
        {
          l.Add(default(T));
        }
      }

      l[index] = data;
    }
  }
}
