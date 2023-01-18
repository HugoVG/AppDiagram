// See https://aka.ms/new-console-template for more information

using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

//Load assembly from path
// Assembly assembly = Assembly.LoadFrom(@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\HRtheGathering\\HRtheGathering\\bin\\Debug\\net6.0\\HRtheGathering.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\\BakaBot\\\BakaBotDiscord\\\bin\\Debug\\net7.0\\BakaBotDiscord.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\Nutrient\\bin\\Debug\\net7.0\\Nutrient.dll");
ModuleDefinition module = ModuleDefinition.ReadModule ("AppContextMaker.dll");
Collection<TypeDefinition> types = module.Types;
//Load all types from assembly
bool allowAnonTypes = false;
AssemblyDefinition[] foo = new[] { module.Assembly };
List<string> colour = new List<string>() { "green", "red", "blue" };
string randomcolour() => colour[Random.Shared.Next(colour.Count)];
//Get all associated assemblies and put it in foo
// foreach(var x in assembly.GetReferencedAssemblies().ToList())
// {
//     if (removeStockAssemblies(x.FullName) == "")
//     {
//         continue;
//     }
//     try
//     {
//         Console.WriteLine(x.Name);
//         foo.Append(Assembly.Load(x));
//
//     }
//     catch (Exception ex)
//     {
//         Console.WriteLine(ex);
//     }
// }
// List<Type> types = new List<Type>();
List<TypeDefinition> safetypes = new List<TypeDefinition>();
Console.WriteLine("direction: right");
foreach(TypeDefinition a in types)
{
    //load all loadable types
     
    //only get types that are not from nuget
    
        
    if (
        // type.IsClass && // I need classes
        // !type.IsAbstract && // Must be able to instantiate the class
        // !type.IsNestedPrivate && // Nested private types are not accessible
        // !type.Assembly.GlobalAssemblyCache && // Excludes most of BCL and third-party classes
        !a.FullName.StartsWith("System.") &&
        !a.FullName.StartsWith("Microsoft.") &&
        !a.FullName.StartsWith("Internal.") &&
        !a.FullName.StartsWith("FxResources.") &&
        !a.FullName.Contains("Migrations.") &&
        a.Namespace != null ) // Yes, it can be null!
        // a.CustomAttributes.Any(x => x is not CompilerGeneratedAttribute)) // Excl. compiler gen.
    {
        safetypes.Add(a);
    }
}

List<string> connections = new List<string>();
foreach (var type in safetypes)
{
    if(type.FullName == "<Module>")
        continue;
    if (!allowAnonTypes && type.FullName.Contains("AnonymousType"))
        continue;
    Console.WriteLine($"{type.FullName.Replace("<>", "").Replace("`", "")} {{");
    if (type.IsClass || type.IsInterface || type.IsValueType)
    {
        Console.WriteLine("\tshape: class");
        // Console.WriteLine("\t" + $@"style: {{
        //         fill: {randomcolour()}
        //         opacity: 1
        //         stroke-width: 2
        //         stroke-dash: 5
        // }}");
    }
    //Check if the type has a parent class
    if (type.BaseType != null)
    {
        if(!type.BaseType.IsPrimitive && !type.BaseType.IsValueType && removeStockAssemblies(type.BaseType.FullName) != "" )
        {
            connections.Add($"{removeStockAssemblies(type.FullName)} -> {removeStockAssemblies(type.BaseType.FullName)} : Inherits");
        }
    }
    //Check if the type implements any interfaces
    foreach (var interfaceType in type.Interfaces)
    {
        if(!interfaceType.InterfaceType.IsPrimitive && !interfaceType.InterfaceType.IsValueType && removeStockAssemblies(interfaceType.InterfaceType.FullName) != "" )
        {
            connections.Add($"{removeStockAssemblies(interfaceType.InterfaceType.FullName)} -> {removeStockAssemblies(type.FullName)} : Implements");
        }
    }
    //Get all methods of the type even private, but not inherited
    if (type.HasMethods)
    {
        Console.WriteLine("\t#Methods:");
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if(method.IsGetter || method.IsSetter)
            {
                continue;
            }
            Console.WriteLine(
                $"\t\"{(method.IsPrivate ? "-" : "+")}{method.Name}({string.Join(", ", method.Parameters.Select(p => $"{typechecker.RecursiveTypeChecker(p.ParameterType)} {p.Name}"))})\" : \"{typechecker.RecursiveTypeChecker(method.MethodReturnType.ReturnType)}\"");
            if(!method.ReturnType.IsPrimitive && 
               !method.ReturnType.IsGenericParameter &&
               type.FullName != typechecker.RecursiveTypeChecker(method.ReturnType) &&
               removeStockAssemblies(method?.ReturnType?.FullName) != null && removeStockAssemblies(method?.ReturnType?.FullName) != "")
                connections.Add($"{type.FullName} -> {typechecker.RecursiveTypeChecker(method.ReturnType)}");
            
        }
    }
    //get all properties of the type even private, but not inherited
    if (type.HasFields)
    {
        
        Console.WriteLine("\t#Fields:");
        
        foreach(var x in type.Fields.ToList().Where(x=> !x.FullName.Contains("k__BackingField")))
        {
            Console.WriteLine($"\t\"{(x.IsPrivate ? "-" : "+")}{x.Name}\" : \"{typechecker.RecursiveTypeChecker(x.FieldType)}\"");
            if(!x.FieldType.IsPrimitive && 
               !x.FieldType.IsGenericParameter &&
               type.FullName != typechecker.RecursiveTypeChecker(x.FieldType) &&
               removeStockAssemblies(x?.FieldType?.FullName) != null && removeStockAssemblies(x?.FieldType?.FullName) != "")
                connections.Add($"{type.FullName} -> {typechecker.RecursiveTypeChecker(x.FieldType)}");
        }
    }

    if (type.HasProperties)
    {
        Console.WriteLine("\t#Properties:");
        
        
        
        type.Properties.ToList().ForEach(x =>
        {
            string getset = "";
            if (x.GetMethod != null)
            {
                getset += "get;";
            }
            if (x.SetMethod != null)
            {
                getset += "set;";
            }
            Console.WriteLine($"\t\"{(x.GetMethod.IsPrivate ? "-" : "+")}{x.Name} {{{getset}}}\" : \"{typechecker.RecursiveTypeChecker(x.PropertyType)}\"");
            // Console.WriteLine($"\t\"{(x.GetMethod.IsPrivate ? "-" : "+")}{x.Name} \" : \"{typechecker.RecursiveTypeChecker(x.PropertyType)}\"");
            if(!x.PropertyType.IsPrimitive && 
               !x.PropertyType.IsGenericParameter &&
               removeStockAssemblies(x?.PropertyType?.FullName) != null && removeStockAssemblies(x?.PropertyType?.FullName) != "")
                connections.Add($"{type.FullName} -> {typechecker.RecursiveTypeChecker(x.PropertyType)}");
        });
    }

    Console.WriteLine("}");
}

Console.WriteLine("#Connections");
foreach (var connection in connections)
{
    Console.WriteLine(connection);
}

string removeStockAssemblies(string s)
{
    string[] lines = new[]
    {
        "System.",
        "Microsoft.",
        "Internal.",
        "FxResources.",
        "Swashbuckle.",
    };
    if(s == null) return "void";
    var contains = lines.Any(x=>s.StartsWith(x));
    return contains ? "" : s;
}

class typechecker
{
    public static string RecursiveTypeChecker(Type type)
    {
        string output = "";

        if (type == null)
        {
            return output;
        }

        if (type.IsGenericType)
            output += $"{type.Name}";
        else
            output += $"{removestocks.removeStockInLine(type.FullName)}";
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

    public static string RecursiveTypeChecker(TypeReference type)
    {
        string output = "";

        if (type == null)
        {
            return output;
        }

        if (type.ContainsGenericParameter)
            output += $"{type.Name}";
        else
            output += $"{removestocks.removeStockInLine(type.FullName)}";
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


class removestocks{
    public static string removeStockInLine(string s)
    {
        string[] lines = new[]
        {
            "System.",
            "Microsoft.",
            "Internal.",
            "FxResources.",
        };
        if(s == null) return "void";
        foreach (var x in lines)
        {
            s = s.Replace(x, "");
        }
        return s;
    }
}
