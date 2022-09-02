using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

public class ProjectParser
{
    /// <summary>
    ///     Parses a project file.
    /// </summary>
    /// <param name="projectFile">The project path.</param>
    /// <param name="configuration"></param>
    /// <param name="runtimeValue"></param>
    /// <returns>The parsed project.</returns>
    public ProjectParseResult Parse(string projectFile,
                                    string configuration,
                                    string runtimeValue)
    {
        // Get the project file.
        var pf = Path.GetTempFileName();
        try
        {
            DotNetTasks.DotNet($"msbuild -preprocess:{pf} {projectFile.DoubleQuoteIfNeeded()}");

            XDocument document = XDocument.Load(pf);

            if (document.Root == null)
                throw new Exception(
                    "Silly error: The parser should never make the root element of a document into a null value");

            var projectProperties = new Dictionary<string, string>();
            projectProperties.Add("Platform", runtimeValue);
            projectProperties.Add("Configuration", configuration);

            // Parsing the build file is sensitive to the declared order of elements.
            // If there are include files, then these files must be honored too.
            ParseProjectProperties(document, configuration, runtimeValue, projectProperties);

            var targetFrameworks = GetValueOrDefault(projectProperties, "TargetFrameworks");
            if (string.IsNullOrEmpty(targetFrameworks))
            {
                targetFrameworks = GetValueOrDefault(projectProperties, "TargetFramework");
            }

            targetFrameworks ??= "";
            var targetFrameworksList = targetFrameworks.Split(";");

            var name = Path.GetFileNameWithoutExtension(projectFile);

            return new ProjectParseResult(name,
                                          GetValueOrDefault(projectProperties, "Configuration"),
                                          GetValueOrDefault(projectProperties, "OutputType", "Library"),
                                          targetFrameworksList);
        }
        finally
        {
            File.Delete(pf);
        }
    }

    string GetValueOrDefault(Dictionary<string, string> d, string key, string value = null)
    {
        string result;
        if (d.TryGetValue(key, out result))
        {
            return ResolveProperty(result, d);
        }

        return value;
    }

    string ResolveProperty(string property, Dictionary<string, string> pool)
    {
        // crude replace 
        foreach (var pair in pool)
        {
            property = property.Replace("$(" + pair.Key + ")", pair.Value);
        }

        return property;
    }

    bool MatchesCondition(XElement propertyGroup, string configuration, string platform)
    {
        var value = (string)propertyGroup.Attribute("Condition");
        if (string.IsNullOrEmpty(value))
            return true;
        if (value.Contains(string.Concat("'$(Configuration)|$(Platform)' == '", configuration, "|", platform, "'")))
            return true;
        if (value.Contains(string.Concat("'$(Configuration)' == '", configuration, "'")))
            return true;
        if (value.Contains(string.Concat("'$(Platform)' == '", platform, "'")))
            return true;
        return false;
    }

    void ParseProjectProperties(XDocument document, string configuration, string platform,
                                Dictionary<string, string> rawProperties)
    {
        if (document.Root == null)
        {
            return;
        }
        
        foreach (var element in document.Root.Elements())
        {
            if (element.Name.LocalName == "PropertyGroup")
            {
                if (!MatchesCondition(element, configuration, platform))
                    continue;

                foreach (var prop in element.Elements())
                {
                    var cond = (string)prop.Attribute("Condition");
                    if (!string.IsNullOrEmpty(cond))
                        continue;

                    var name = prop.Name.LocalName;
                    var value = (string)prop;
                    rawProperties[name] = value;
                }
            }
        }
    }
}
