using System.Collections.Generic;

/// <summary>Represents the content in an MSBuild project file.</summary>
public class ProjectParseResult
{
    public string Configuration { get; }
    public string OutputType { get; }
    public string Name { get; }
    public IReadOnlyList<string> TargetFrameworks { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="T:Cake.Common.Solution.Project.ProjectParserResult" /> class.
    /// </summary>
    public ProjectParseResult(string name, 
                              string configuration,
                              string outputType,
                              string[] targetFrameworks)
    {
        Name = name;
        Configuration = configuration;
        OutputType = outputType;
        TargetFrameworks = targetFrameworks;
    }

}
