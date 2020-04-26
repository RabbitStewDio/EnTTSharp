using System;
using EnttSharp.Entities.Data;

namespace EnttSharp.Entities
{
  /// <summary>
  ///  Specialisation to avoid boxing when requesting an enumerator.
  /// </summary>
  public abstract class AdhocMultiViewBase : MultiViewBase<PredicateEnumerator<EntityKey>>
  {
    protected AdhocMultiViewBase(EntityRegistry registry,
                                 params ISparsePool[] entries): base(registry, entries)
    {
    }

    protected override int EstimatedSize
    {
        get
        {
            if (Sets.Count == 0)
            {
                return 0;
            }
            
            var count = int.MaxValue;
            foreach (var set in Sets)
            {
                if (set.Count < count)
                {
                    count = set.Count;
                }
            }

            return count;
        }
    }

    public override PredicateEnumerator<EntityKey> GetEnumerator()
    {
      ISparsePool s = null;
      var count = int.MaxValue;
      foreach (var set in Sets)
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
