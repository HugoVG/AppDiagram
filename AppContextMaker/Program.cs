using System.Diagnostics;
using System.Net.Sockets;
using AppContextMaker.Extensions;
using AppContextMaker.Helpers;
using AppContextMaker.Packages;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

//Load assembly from path
// Assembly assembly = Assembly.LoadFrom(@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\HRtheGathering\\HRtheGathering\\bin\\Debug\\net6.0\\HRtheGathering.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\\BakaBot\\\BakaBotDiscord\\\bin\\Debug\\net7.0\\BakaBotDiscord.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\Nutrient\\bin\\Debug\\net7.0\\Nutrient.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule ("AppContextMaker.dll");
string dllLocation = "C:\\Users\\megah\\Downloads\\Concert_Overlay_Windows_1.1.0\\Concert_Overlay_Windows_1.1.0\\Concert Overlay_Data\\Managed\\Assembly-CSharp.dll";
ModuleDefinition module = ModuleDefinition.ReadModule (dllLocation);
Collection<TypeDefinition> types = module.Types;
//Load all types from assembly
bool allowAnonTypes = false;
//Get all associated assemblies and put it in foo
List<TypeDefinition> safetypes = new List<TypeDefinition>();
// Console.WriteLine("direction: right");
var removestocks = new StringHelpers();
foreach(TypeDefinition a in types)
{
    //load all loadable types
    if (!a.FullName.ContainsAny(removestocks.LeftOutNameSpaces) &&
        a.Namespace != null && // Yes, it can be null!
        !CecilHelpers.HasCompilerGeneratedAttribute(a)// Excl. compiler gen.
        ) 
    {
        safetypes.Add(a);
    }
}

IDiagram package = new D2Package();
package.Start();





//TODO: Remove this
List<string> connections = new List<string>();
foreach (var type in safetypes)
{
    if(type.FullName == "<Module>")
        continue;
    if (!allowAnonTypes && type.FullName.Contains("AnonymousType"))
        continue;
    
    //Console.WriteLine($"{StringHelpers.RemoveAnonymousTypes(type.FullName)} {{");
    Class @class = new Class();
    @class.Name = removestocks.RemoveStockInLine(type.FullName);
    
    if (type.IsClass || type.IsInterface || type.IsValueType)
    {
        //Console.WriteLine("\tshape: class");
    }
    //Check if the type has a parent class
    if (type.BaseType != null)
    {
        if(!type.BaseType.IsPrimitive && !type.BaseType.IsValueType && removeStockAssemblies(type.BaseType.FullName) != "void" && removeStockAssemblies(type.BaseType.FullName) != "" )
        {
            
            package.MakeTypedConnections(type, type?.BaseType, "Inherits");
            //connections.Add($"{removestocks.RemoveStockInLine(type.FullName)} -> {removestocks.RemoveStockInLine(type.BaseType.FullName)} : Inherits");
        }
    }
    //Check if the type implements any interfaces
    foreach (var interfaceType in type.Interfaces)
    {
        if(!interfaceType.InterfaceType.IsPrimitive && !interfaceType.InterfaceType.IsValueType && removeStockAssemblies(interfaceType.InterfaceType.FullName) != "" )
        {
            package.MakeStringConnections(type.FullName, type.BaseType.FullName, "Implements");
            //connections.Add($"{removestocks.RemoveStockInLine(interfaceType.InterfaceType.FullName)} -> {removestocks.RemoveStockInLine(type.FullName)} : Implements");
        }
    }
    //Get all methods of the type even private, but not inherited
    if (type.HasMethods)
    {
        //Console.WriteLine("\t#Methods:");
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if(method.IsGetter || method.IsSetter || method.IsConstructor) //skip properties
            {
                continue;
            }
            @class.Methods.Add(new Method()
            {
                Name = method.Name,
                Parameters = method.Parameters.ToList(),
                Visibility = CecilHelpers.GetVisibility(method),
                ReturnType = method.ReturnType
            });
            
            // Console.WriteLine($"\t\"{(method.IsPrivate ? "-" : "+")}{method.Name}({string.Join(", ", method.Parameters.Select(p => $"{removestocks.RecursiveTypeChecker(p.ParameterType)} {p.Name}"))})\" : \"{removestocks.RecursiveTypeChecker(method.MethodReturnType.ReturnType)}\"");
            if(!method.ReturnType.IsPrimitive && 
               !method.ReturnType.IsGenericParameter &&
               type.FullName != removestocks.RecursiveTypeChecker(method.ReturnType) &&
               removeStockAssemblies(method?.ReturnType?.FullName) != null && removeStockAssemblies(method?.ReturnType?.FullName) != "")
            {
                package.MakeTypedConnections(type, method.ReturnType);
                connections.Add($"{type.FullName} -> {removestocks.RecursiveTypeChecker(method.ReturnType)}");
            }
        }
    }
    //get all properties of the type even private, but not inherited
    if (type.HasFields)
    {
        
        // Console.WriteLine("\t#Fields:");
        
        foreach(var x in type.Fields.ToList().Where(x=> !x.FullName.Contains("k__BackingField")))
        {
            @class.Fields.Add(new Field()
            {
                Name = x.Name,
                Type = removestocks.RecursiveTypeChecker(x.FieldType),
                TypeDefinition = x.DeclaringType,
                Visibility = CecilHelpers.GetVisibility(x)
            });
            // Console.WriteLine($"\t\"{(x.IsPrivate ? "-" : "+")}{x.Name}\" : \"{removestocks.RecursiveTypeChecker(x.FieldType)}\"");
            if (!x.FieldType.IsPrimitive &&
                !x.FieldType.IsGenericParameter &&
                type.FullName != removestocks.RecursiveTypeChecker(x.FieldType) &&
                removeStockAssemblies(x?.FieldType?.FullName) != null &&
                removeStockAssemblies(x?.FieldType?.FullName) != "")
            {
                package.MakeTypedConnections(type, x.FieldType);
                // connections.Add($"{type.FullName} -> {removestocks.RecursiveTypeChecker(x.FieldType)}");
                
            }
        }
    }

    if (type.HasProperties)
    {
        // Console.WriteLine("\t#Properties:");
        type.Properties.ToList().ForEach(x =>
        {
            
            @class.Properties.Add(new Property
            {
                IsGet = x.GetMethod != null,
                IsSet = x.SetMethod != null,
                Name = x.Name,
                Type = removestocks.RecursiveTypeChecker(x.PropertyType),
                Visibility = CecilHelpers.GetVisibility(x),
                ReturnType = CecilHelpers.RefTypeToObject(x.PropertyType)
            });
            
            string getset = "";
            if (x.GetMethod != null)
            {
                getset += "get;";
            }
            if (x.SetMethod != null)
            {
                getset += "set;";
            }
            // Console.WriteLine($"\t\"{(x.GetMethod.IsPrivate ? "-" : "+")}{x.Name} {{{getset}}}\" : \"{removestocks.RecursiveTypeChecker(x.PropertyType)}\"");
            if(!x.PropertyType.IsPrimitive && 
               !x.PropertyType.IsGenericParameter &&
               removeStockAssemblies(x?.PropertyType?.FullName) != null && removeStockAssemblies(x?.PropertyType?.FullName) != "")
            {
                package.MakeTypedConnections(type, x.PropertyType);
                // connections.Add($"{type.FullName} -> {removestocks.RecursiveTypeChecker(x.PropertyType)}");
            }
        });
    }

    // Console.WriteLine("}");
    package.MakeClass(@class);
}

// Console.WriteLine("#Connections");
// foreach (var connection in connections)
// {
//     
//     Console.WriteLine(StringHelpers.EscapeExcept(connection, ".: ->"));
// }
package.End();



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

