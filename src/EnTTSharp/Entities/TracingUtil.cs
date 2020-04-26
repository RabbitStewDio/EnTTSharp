using System;
using System.Diagnostics;

namespace EnttSharp.Entities
{
  internal static class TracingUtil
  {
    public static TraceSource Create<T>(SourceLevels level = SourceLevels.All)
    {
      return new TraceSource(NameWithoutGenerics(typeof (T)), level);
    }

    public static TraceSource Create(Type t, SourceLevels level = SourceLevels.All)
    {
      return new TraceSource(NameWithoutGenerics(t), level);
    }

    static string NameWithoutGenerics(Type t)
    {
      string str = t.FullName ?? t.Name;
      int length = str.IndexOf('`');
      if (length != -1)
        return str.Substring(0, length);
      return str;
    }
  }
}
