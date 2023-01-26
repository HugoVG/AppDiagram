using System.Text.RegularExpressions;
using AppContextMaker.Packages;
using Mono.Cecil;

namespace AppContextMaker.Helpers;

public static class CecilHelpers
{
    public static bool HasCompilerGeneratedAttribute(TypeDefinition type)
    {
        if (!type.HasCustomAttributes)
            return false;
        foreach (CustomAttribute attribute in type.CustomAttributes)
        {
            if (attribute.AttributeType.Name == "CompilerGeneratedAttribute")
                return true;
        }
        return false;
    }
    public static Visibility GetVisibility(FieldDefinition field)
    {
        if (field.IsPublic)
            return Visibility.Public;
        if (field.IsPrivate)
            return Visibility.Private;
        if (field.IsFamily)
            return Visibility.Protected;
        return Visibility.Public;
    }
    public static Visibility GetVisibility(MethodDefinition method)
    {
        if (method.IsPublic)
            return Visibility.Public;
        if (method.IsPrivate)
            return Visibility.Private;
        if (method.IsFamily)
            return Visibility.Protected;
        return Visibility.Public;
    }
    public static Visibility GetVisibility(PropertyDefinition property)
    {
        if (property.GetMethod.IsPublic)
            return Visibility.Public;
        if (property.GetMethod.IsPrivate)
            return Visibility.Private;
        if (property.GetMethod.IsFamily)
            return Visibility.Protected;
        return Visibility.Public;
    }
    public static Objects RefTypeToObject(TypeReference type)
    {
        try
        {
            return new Objects
            {
                Name = type.Name,
                TypeDefinition = type.Resolve(),
                Type = RecursiveTypeChecker(type)
            };
        }
        catch (Exception ex)
        {
            return new Objects
            {
                Name = type.Name,
                Type = RecursiveTypeChecker(type)
            };
        }
    }
    private static string RecursiveTypeChecker(TypeReference type)
    {
        var i = new StringHelpers();
        
        string output = "";

        if (type == null)
        {
            return output;
        }

        if (type.ContainsGenericParameter)
            output += $"{type.Name}";
        else
            output += $"{i.RemoveStockInLine(type.FullName)}";
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