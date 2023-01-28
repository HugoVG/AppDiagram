using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AppExplorer.Helpers;
using Mono.Cecil;

namespace AppExplorer.Packages;

public class MDPackage : IDiagram
{
    private readonly StringHelpers removestocks = new();

    public MDPackage(StringHelpers _removestocks = null)
    {
        removestocks = _removestocks ?? new();
    }

    public List<string> AllTexts { get; set; } = new List<string>();
    public List<Class> Classes { get; set; } = new List<Class>();
    public List<string> Connections { get; set; }

    public void MakeClass(Class _class)
    {
        Classes.Add(_class);
    }

    public void MakeTypedConnections(TypeDefinition FromObject, TypeReference ToObject, string text = "")
    {
    }


    public string Package()
    {
        var namespaceBound = Classes.GroupBy(x => x.Namespace);

        foreach (var _namespace in namespaceBound)
        {
            AllTexts.Add("# " + _namespace.Key);
            foreach (var _class in _namespace)
            {
                MakeMarkdownFromClass(_class);
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (var text in AllTexts)
        {
            sb.AppendLine(text);
        }

        return sb.ToString();
    }

    public void Start(Direction direction = Direction.Right)
    {
        //Disregard Direction
    }

    public string End(string ModuleName = "Package")
    {
        //Create folder in program files
        Directory.CreateDirectory("Markdown");
        Directory.CreateDirectory("Markdown/Wiki");
        Directory.CreateDirectory($"Markdown/Wiki/{ModuleName}");
        var namespaceBound = Classes.GroupBy(x => x.Namespace);
        foreach (var _namespace in namespaceBound)
        {
            AllTexts.Add("# " + _namespace.Key);
            foreach (var _class in _namespace)
            {
                MakeMarkdownFromClass(_class);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var text in AllTexts)
            {
                sb.AppendLine(text);
            }

            string package = sb.ToString();
            string className = _namespace.Key;
            if (className.Contains("."))
                className = className.Split(".").Last();

            File.WriteAllText($"Markdown/Wiki/{ModuleName}/{className}.md", package);
            AllTexts = new List<string>();
        }

        string packagename = namespaceBound.First().Key.Split(".").First();
        //Open File explorer to the folder

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            Process.Start("explorer.exe", Path.GetFullPath($"Markdown/Wiki/{ModuleName}"));

        return packagename;
    }

    private void MakeMarkdownFromClass(Class _class)
    {
        //Adds the class name which is after the last dot
        string className = _class.Name;
        if (className.Contains("."))
            className = className.Split(".").Last();

        AllTexts.Add($"## {className}");
        ResolveFields(_class);

        ResolveProperties(_class);

        ResolveMethods(_class);
    }

    private void ResolveMethods(Class _class)
    {
        if (_class.Methods.Any())
        {
            if (_class.Properties.Any() || _class.Fields.Any())
                AllTexts.Add("");
            //Do the same for methods
            AllTexts.Add($"### Methods:");
            AllTexts.Add("| Name  | Parameters | Return Type |");
            AllTexts.Add("| ---  | --- | --- |");
            foreach (var method in _class.Methods)
            {
                string text =
                    $"| {method.Name.Replace("<", "")} | {string.Join(", ", method.Parameters.Select(p => $"``{removestocks.RecursiveTypeChecker(p.ParameterType)}`` {p.Name}"))} | {removestocks.RecursiveTypeChecker(method.ReturnType)} |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                AllTexts.Add(text);
            }
        }
    }

    private void ResolveProperties(Class _class)
    {
        if (_class.Properties.Any())
        {
            if (_class.Fields.Any())
                AllTexts.Add("");
            AllTexts.Add($"### Properties:");
            AllTexts.Add("| Name | Type |");
            AllTexts.Add("| --- | --- |");
            foreach (var property in _class.Properties)
            {
                string text =
                    $"| {property.Name.Replace("<", "")} {StringHelpers.PropertyGetSetToString(property).Replace("{", "").Replace("}", "").Replace(";", "")} | ``{property.Type}`` |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                AllTexts.Add(text);
            }
        }
    }

    private void ResolveFields(Class _class)
    {
        if (_class.Fields.Any())
        {
            AllTexts.Add($"### Fields:");
            AllTexts.Add("| Name | Type |");
            AllTexts.Add("| --- | --- |");
            foreach (var field in _class.Fields)
            {
                string text =
                    $"| {field.Name.Replace("<", "")} | ``{removestocks.RecursiveTypeChecker(field.TypeDefinition)}`` |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                AllTexts.Add(text);
            }
        }
    }

    private string removeUnderscores(string text)
    {
        return text.Replace("_", "\\_");
    }
}