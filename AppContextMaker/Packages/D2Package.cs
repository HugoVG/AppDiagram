using System.Diagnostics;
using System.Text;
using AppContextMaker.Helpers;
using Mono.Cecil;

namespace AppContextMaker.Packages;

public class D2Package : IDiagram
{
    private readonly StringHelpers removestocks = new();
    public D2Package(StringHelpers _removestocks = null)
    {
        removestocks = _removestocks ?? new();
    }
    public List<string> AllTexts { get; set; } = new List<string>();
    

    public List<Class> Classes { get; set; } = new List<Class>();
    public List<string> Connections { get; set; } = new List<string>();
    public string Package()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var text in AllTexts)
        {
            sb.AppendLine(text);
        }

        sb.AppendLine("#Connections");
        foreach (var connection in Connections)
        {
            sb.AppendLine(StringHelpers.EscapeExcept(connection, ".: ->"));
        }

        return sb.ToString();
    }
    
    public void Start()
    {
        AllTexts.Add("direction: right");
    }

    /// <summary>
    /// Starts d2 to make the diagram
    /// </summary>
    public void End()
    {
        // File.WriteAllText("Package.d2", Package());
        Directory.CreateDirectory("Diagrams");
        
        Directory.CreateDirectory("Diagrams/D2");
        Directory.CreateDirectory("Diagrams/SVG");
        File.WriteAllText("Diagrams/D2/Package.d2", Package());
        
        if(File.Exists("d2.exe"))
        {
            var process = Process.Start("d2.exe", "Diagrams/D2/Package.d2 Diagrams/SVG/Package.svg");
            process.WaitForExit(5000);
            Console.WriteLine("Completed diagram");
        }
    }

    public void MakeClass(Class _class)
    {
        AllTexts.Add($"{StringHelpers.RemoveAnonymousTypes(_class.Name)} {{");
        AllTexts.Add("\tshape: class");
        
        AllTexts.Add("#Fields:");
        foreach (var field in _class.Fields)
        {
            AllTexts.Add($"\t\"{VisibilityToString(field.Visibility)}{field.Name}\" : \"{removestocks.RecursiveTypeChecker(field.TypeDefinition)}\"");
        }
        
        AllTexts.Add("#Properties:");

        foreach (var property in _class.Properties)
        {
            AllTexts.Add($"\t\"{VisibilityToString(property.Visibility)}{property.Name} {PropertyGetSetToString(property)}\" : \"{removestocks.RecursiveTypeChecker(property.TypeDefinition)}\"");
        }
        AllTexts.Add("#Methods:");
        foreach (var method in _class.Methods)
        {
            AllTexts.Add($"\t\"{VisibilityToString(method.Visibility)}{method.Name}({string.Join(", ", method.Parameters.Select(p => $"{removestocks.RecursiveTypeChecker(p.ParameterType)} {p.Name}"))})\" : \"{removestocks.RecursiveTypeChecker(method.ReturnType)}\"");
        }
        AllTexts.Add("}");
    }
    public void MakeConnections(Objects FromObject, Objects ToObject, string text = "")
    {
        if(FromObject == null || ToObject == null)
            return;
        string connection = $"{FromObject.TypeDefinition.FullName} -> {removestocks.RecursiveTypeChecker(ToObject.TypeDefinition)} {(text == "" ? "" : $": {text}")}";
        Connections.Add(connection);
    }

    public void MakeStringConnections(string FromObject, string ToObject, string text = "")
    {
        if(FromObject == null || ToObject == null)
            return;
        string connection = $"{removestocks.RemoveStockInLine(FromObject)} -> {removestocks.RemoveStockInLine(ToObject)} {(text == "" ? "" : $": {text}")}";
        Connections.Add(connection);
    }

    public void MakeTypedConnections(TypeDefinition FromObject, TypeReference ToObject, string text = "")
    {
        if(FromObject == null || ToObject == null)
            return;
        string connection = $"{FromObject.FullName} -> {removestocks.RecursiveTypeChecker(ToObject)} {(text == "" ? "" : $": {text}")}";
        Connections.Add(connection);
    }

    private string VisibilityToString(Visibility visibility)
    {
        return visibility switch
        {
            Visibility.Public => "+",
            Visibility.Private => "-",
            Visibility.Protected => "#",
            _ => "+"
        };
    }
    private string ObjectTypeToString(ObjectType objectType)
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
    private string PropertyGetSetToString(Property propertyGetSet)
    {
        return propertyGetSet.IsGet ? propertyGetSet.IsSet ? "{get; set;}" : "{get;}" : "{set;}";
    }
}