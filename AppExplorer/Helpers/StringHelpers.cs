using System.Text.RegularExpressions;
using AppExplorer.Extensions;
using AppExplorer.Packages;
using Mono.Cecil;

namespace AppExplorer.Helpers;

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
        return Regex.Replace(input, $"[^{exception}\\w\\d]", match =>
        {
            return Regex.Escape(match.Value)
                    .Replace("]", "\\]")
                    .Replace("<", "\\<")
                    .Replace(">", "\\>")
                ;
        });
    }

    public static bool RemoveSystemTypes(TypeDefinition FromObject, TypeReference ToObject)
    {
        if (ToObject.FullName.Contains("System") || ToObject.FullName.Contains("Microsoft") ||
            FromObject.FullName.Contains("System") || FromObject.FullName.Contains("Microsoft"))
        {
            return true;
        }

        return false;
    }

    private const string VoidKeyword = "void"; 

    public static bool RemoveVoidsAndEmpty(string fromString, string toString) => IsEmptyOrVoid(fromString) || IsEmptyOrVoid(toString);

    public static string ObjectTypeToString(ObjectType objectType)
    {
        return objectType switch
        {
            ObjectType.Class => "class",
            ObjectType.Interface => "interface",
            ObjectType.Struct => "struct",
            ObjectType.Enum => "enum",
            _ => "class"
        };
    }

    public static string PropertyGetSetToString(Property propertyGetSet)
    {
        return propertyGetSet.IsGet ? propertyGetSet.IsSet ? "{get; set;}" : "{get;}" : "{set;}";
    }

    public static string RemoveGeneric(string toString)
    {
        if (!toString.Contains("<")) return toString;
        toString = toString.Substring(0, toString.IndexOf("<", StringComparison.Ordinal));
        //remove ">" if it exists
        if (toString.EndsWith(">"))
            toString = toString.Replace(">", "");

        return toString;
    }

    public static string RemoveAnonymousTypes(string input)
    {
        return input.Replace("<>", "").Replace("`", "");
    }

    public string RemoveStockInLine(string? s) => s == null ? "void" : s.ReplaceForbiddenWords(LeftOutNameSpaces.ToArray());

    public static bool IsEmptyString(string s) => s == "";

    public static bool IsVoidString(string s) =>
        string.Equals(s, VoidKeyword, StringComparison.InvariantCultureIgnoreCase);

    public static bool IsEmptyOrVoid(string s) => IsEmptyString(s) || IsVoidString(s);

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