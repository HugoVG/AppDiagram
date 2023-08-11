using AppExplorer.Extensions;
using AppExplorer.Helpers;
using AppExplorer.Packages;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace AppExplorer;

public class TypeDefiner
{
    public string ReadDllTypes(string dllLocation, IDiagram package, List<string> leftOutNameSpaces = null,
        Direction direction = Direction.Right)
    {
        ModuleDefinition module = ModuleDefinition.ReadModule(dllLocation);
        string moduleName = module.Name;
        // remove the extension from the module name
        moduleName = moduleName.Substring(0, moduleName.LastIndexOf('.'));
        //Get all types from assembly
        Collection<TypeDefinition> types = module.Types;

        //all types that are not from the .NET framework
        List<TypeDefinition> safetypes = new List<TypeDefinition>();
        var removestocks = new StringHelpers();
        bool allowAnonTypes = false;
        if (leftOutNameSpaces != null)
        {
            removestocks.AddLeftOutNameSpaces(leftOutNameSpaces.ToArray());
        }

        OnlyGetUserMadeTypes(types, removestocks, safetypes);
        package.Start(direction);
        foreach (var type in safetypes)
        {
            if (type.FullName == "<Module>")
                continue;
            if (!allowAnonTypes && type.FullName.Contains("AnonymousType"))
                continue;

            Class @class = new Class();
            @class.Name = removestocks.RemoveStockInLine(type.FullName);
            @class.Namespace = type.Namespace;
            //Check if the type has a parent class
            ResolveInheritance(type, package, removestocks);
            //Check if the type implements any interfaces
            ResolveInterfaces(type, package, removestocks);
            //Get all methods of the type even private, but not inherited
            ResolveMethods(type, @class, removestocks, package);
            //get all properties of the type even private
            ResolveFields(type, @class, removestocks, package);
            //Get all fields
            ResolveProperties(type, @class, removestocks, package);
            //Embed the class in the package
            package.MakeClass(@class);
        }
        //create the package and embed it in the diagram

        return package.End(moduleName);
    }

    private static void OnlyGetUserMadeTypes(Collection<TypeDefinition> types, StringHelpers removestocks,
        List<TypeDefinition> safetypes)
    {
        foreach (TypeDefinition a in types)
        {
            //load all loadable types
            if (!a.FullName.ContainsAny(removestocks.LeftOutNameSpaces) &&
                a.Namespace != null && // Yes, it can be null!
                !CecilHelpers.HasCompilerGeneratedAttribute(a) // Excl. compiler gen.
               )
            {
                safetypes.Add(a);
            }
        }
    }

    private void ResolveInheritance(TypeDefinition type, IDiagram package, StringHelpers removestocks)
    {
        if (type.BaseType == null) return;
        if (!type.BaseType.IsPrimitive && !type.BaseType.IsValueType &&
            removestocks.RemoveStockInLine(type.BaseType.FullName) != "void" &&
            removestocks.RemoveStockInLine(type.BaseType.FullName) != "")
        {
            package.MakeTypedConnections(type, type?.BaseType, "Inherits");
        }
    }

    private void ResolveInterfaces(TypeDefinition type, IDiagram package, StringHelpers removestocks)
    {
        foreach (var interfaceType in type.Interfaces)
        {
            if (!interfaceType.InterfaceType.IsPrimitive && !interfaceType.InterfaceType.IsValueType &&
                removestocks.RemoveStockInLine(interfaceType.InterfaceType.FullName) != "")
            {
                if (interfaceType.InterfaceType.FullName.Contains("Object"))
                    return;
                package.MakeTypedConnections(type, type.BaseType, "Implements");
            }
        }
    }

    private static void ResolveMethods(TypeDefinition type, Class @class, StringHelpers removestocks, IDiagram package)
    {
        if (!type.HasMethods) return;
        var methods = type.GetMethods();
        foreach (var method in methods)
        {
            if (method.IsGetter || method.IsSetter || method.IsConstructor) //skip properties
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

            if (!method.ReturnType.IsPrimitive &&
                !method.ReturnType.IsGenericParameter &&
                type.FullName != removestocks.RecursiveTypeChecker(method.ReturnType) &&
                removestocks.RemoveStockInLine(method?.ReturnType?.FullName) != null &&
                removestocks.RemoveStockInLine(method?.ReturnType?.FullName) != "")
            {
                package.MakeTypedConnections(type, method.ReturnType);
            }
        }
    }

    private static void ResolveFields(TypeDefinition type, Class @class, StringHelpers removestocks, IDiagram package)
    {
        if (!type.HasFields) return;
        foreach (var x in type.Fields.ToList().Where(x => !x.FullName.Contains("k__BackingField")))
        {
            @class.Fields.Add(new Field()
            {
                Name = x.Name,
                Type = removestocks.RecursiveTypeChecker(x.FieldType),

                TypeDefinition = x.DeclaringType,
                Visibility = CecilHelpers.GetVisibility(x)
            });
            if (!x.FieldType.IsPrimitive &&
                !x.FieldType.IsGenericParameter &&
                type.FullName != removestocks.RecursiveTypeChecker(x.FieldType) &&
                removestocks.RemoveStockInLine(x?.FieldType?.FullName) != null &&
                removestocks.RemoveStockInLine(x?.FieldType?.FullName) != "")
            {
                package.MakeTypedConnections(type, x.FieldType);
            }
        }
    }

    private void ResolveProperties(TypeDefinition type, Class @class, StringHelpers RemoveBuiltInTypes, IDiagram package)
    {
        if (!type.HasProperties) return;
        // Console.WriteLine("\t#Properties:");
        type.Properties.ToList().ForEach(x =>
        {
            @class.Properties.Add(new Property
            {
                IsGet = x.GetMethod != null,
                IsSet = x.SetMethod != null,
                Name = x.Name,
                Type = RemoveBuiltInTypes.RecursiveTypeChecker(x.PropertyType),
                Visibility = CecilHelpers.GetVisibility(x),
                ReturnType = CecilHelpers.RefTypeToObject(x.GetMethod.ReturnType)
            });
            string? fullname = x?.PropertyType?.FullName;
            if (!x.PropertyType.IsPrimitive &&
                !x.PropertyType.IsGenericParameter &&
                RemoveBuiltInTypes.RemoveStockInLine(fullname) != null &&
                RemoveBuiltInTypes.RemoveStockInLine(fullname) != "")
            {
                package.MakeTypedConnections(type, x.PropertyType);
            }
        });
    }
}