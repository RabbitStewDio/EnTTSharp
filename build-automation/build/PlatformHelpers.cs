using System.Collections.Generic;
using System.Linq;

public static class PlatformHelpers
{
    static readonly HashSet<string> netFrameworkIds = new HashSet<string>()
    {
        "net11",
        "net20",
        "net35",
        "net40",
        "net403",
        "net45",
        "net451",
        "net452",
        "net46",
        "net461",
        "net462",
        "net47",
        "net471",
        "net472",
        "et48"
    };

    public static bool IsValidPlatformFor(this ProjectParseResult p, string targetValue)
    {
        var targetFrameworks = p.TargetFrameworks;
        if (targetValue.StartsWith("win"))
        {
            // filter out known windows-only targets.
            // That is both the old .NET framework and 
            // the newer net5.0-windows handle.
            if (targetFrameworks.Any(f => f.EndsWith("-windows")) ||
                targetFrameworks.Any(f => netFrameworkIds.Contains(f)))
            {
                return true;
            }
        }

        return true;
    }
}
