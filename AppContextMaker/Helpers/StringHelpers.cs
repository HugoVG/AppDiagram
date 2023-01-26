using System.Text.RegularExpressions;
using AppContextMaker.Extensions;
using Mono.Cecil;

namespace AppContextMaker.Helpers;

public class StringHelpers
{
    public List<string> LeftOutNameSpaces = new()
    {
        "System.",
        "Microsoft.",
        "Internal.",
        "FxResources.",
    };
    public void AddLeftOutNameSpaces(string nameSpace)
    {
        LeftOutNameSpaces.Add(nameSpace);
    }
    public void AddLeftOutNameSpaces(params string[] nameSpaces)
    {
        foreach (var nameSpace in nameSpaces)
        {
            LeftOutNameSpaces.Add(nameSpace);
        }
    }
    
    public static string EscapeExcept(string input, string exception)
    {
        return Regex.Replace(input, $"[^{exception}\\w\\d]", match => {
            return Regex.Escape(match.Value).Replace("]", "\\]");
        });
    }
    public static string RemoveAnonymousTypes(string input)
    {
        return input.Replace("<>", "").Replace("`", "");
    }
    public string RemoveStockInLine(string s)
    {
        if(s == null) return "void";
        return s.ReplaceForbiddenWords(LeftOutNameSpaces.ToArray());
    }
    public string RecursiveTypeChecker(Type type)
    {
        string output = "";

        if (type == null)
        {
            return output;
        }

        if (type.IsGenericType)
            output += $"{type.Name}";
        else
            output += $"{RemoveStockInLine(type.FullName)}";
        var generics = type.GetGenericArguments();
        foreach (var var in generics)
        {
            var typing = RecursiveTypeChecker(var);
            if (typing != "")
            {
                output += $"<{typing}>";

            }
        }
        output = output.Replace("><", ", ");
        output = Regex.Replace(output, "`(\\d)+", "");
        return output;
    }

    public string RecursiveTypeChecker(TypeReference type)
    {
        string output = "";

        if (type == null)
        {
            return output;
        }

        if (type.ContainsGenericParameter)
            output += $"{type.Name}";
        else
            output += $"{RemoveStockInLine(type.FullName)}";
        var generics = type.GenericParameters;
        foreach (var var in generics)
        {
            var typing = RecursiveTypeChecker(var);
            if (typing != "")
            {
                output += $"<{typing}>";

            }
        }

        output = output.Replace("><", ", ");
        output = Regex.Replace(output, "`(\\d)+", "");
        return output;
    }
}