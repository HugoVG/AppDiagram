using System.Diagnostics;
using System.Text;
using AppExplorer.Helpers;
using Mono.Cecil;

namespace AppExplorer.Packages;

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
        //Should only do Distinct otherwise it will make a lot of connections
        foreach (var connection in Connections.Distinct())
        {
            sb.AppendLine(StringHelpers.EscapeExcept(connection, ".: ->"));
        }

        return sb.ToString();
    }

    public void Start(Direction direction = Direction.Right)
    {
        AllTexts.Add($"direction: {DirectionToString(direction)}");
    }

    /// <summary>
    /// Starts d2 to make the diagram
    /// </summary>
    public string End(string ModuleName = "Package")
    {
        Directory.CreateDirectory("Diagrams");

        Directory.CreateDirectory("Diagrams/D2");
        Directory.CreateDirectory("Diagrams/SVG");
        File.WriteAllText($"Diagrams/D2/{ModuleName}.d2", Package());
        if (!File.Exists("d2.exe")) return ""; // If d2.exe is not found, then don't do anything
        var process = Process.Start("d2.exe", $"Diagrams/D2/{ModuleName}.d2 Diagrams/SVG/{ModuleName}.svg");
        process.WaitForExit(180000); // 3 minute
        if (File.Exists($"Diagrams/SVG/{ModuleName}.svg"))
        {
            //Get absolute path to the file
            var path = Path.GetFullPath($"Diagrams/SVG/{ModuleName}.svg");
            return path;
        }

        return $"Diagrams/SVG/{ModuleName}.svg";
    }

    public void MakeClass(Class _class)
    {
        AllTexts.Add($"{StringHelpers.RemoveAnonymousTypes(_class.Name)} {{");
        AllTexts.Add("\tshape: class");

        AllTexts.Add("#Fields:");
        foreach (var field in _class.Fields)
        {
            AllTexts.Add(
                $"\t\"{VisibilityToString(field.Visibility)}{field.Name}\" : \"{removestocks.RecursiveTypeChecker(field.TypeDefinition)}\"");
        }

        AllTexts.Add("#Properties:");

        foreach (var property in _class.Properties)
        {
            AllTexts.Add(
                $"\t\"{VisibilityToString(property.Visibility)}{property.Name} {StringHelpers.PropertyGetSetToString(property)}\" : \"{removestocks.RecursiveTypeChecker(property.ReturnType.TypeDefinition)}\"");
        }

        AllTexts.Add("#Methods:");
        foreach (var method in _class.Methods)
        {
            AllTexts.Add(
                $"\t\"{VisibilityToString(method.Visibility)}{method.Name}({string.Join(", ", method.Parameters.Select(p => $"{removestocks.RecursiveTypeChecker(p.ParameterType)} {p.Name}"))})\" : \"{removestocks.RecursiveTypeChecker(method.ReturnType)}\"");
        }

        AllTexts.Add("}");
    }

    public void MakeTypedConnections(TypeDefinition FromObject, TypeReference ToObject, string text = "")
    {
        if (FromObject == null || ToObject == null)
            return;
        string fromString = FromObject.FullName;
        string toString = removestocks.RecursiveTypeChecker(ToObject);
        if (StringHelpers.RemoveVoidsAndEmpty(fromString, toString)) return;
        //if ToObject contains "System" or "Microsoft" then we need to remove it
        if (StringHelpers.RemoveSystemTypes(FromObject, ToObject)) return;
        //if toString is a generic type, we need to remove the generic type
        toString = StringHelpers.RemoveGeneric(toString);
        string connection = $"{fromString} -> {toString} {(text == "" ? "" : $": {text}")}";
        //if connection already exists, we don't need to add it again
        if (Connections.Contains(connection))
            return;
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

    private string DirectionToString(Direction direction)
    {
        return direction switch
        {
            Direction.Left => "left",
            Direction.Right => "right",
            Direction.Up => "up",
            Direction.Down => "down",
            _ => "right"
        };
    }
}