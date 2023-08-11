using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AppExplorer.Helpers;
using Mono.Cecil;

namespace AppExplorer.Packages;

public class MDPackage : IDiagram
{
    /// <summary>
    /// Types to be removed otherwise you'll get issues with class X being dependant on System.String and System.Object
    /// </summary>
    private readonly StringHelpers CSharpInternalTypes = new();

    public MDPackage(StringHelpers _removestocks = null)
    {
        CSharpInternalTypes = _removestocks ?? new();
    }

    public List<string> RenderableText { get; set; } = new();
    public List<Class> Classes { get; set; } = new();
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
            RenderableText.Add("# " + _namespace.Key);
            foreach (var _class in _namespace)
            {
                MakeMarkdownFromClass(_class);
            }
        }

        StringBuilder sb = new StringBuilder();
        foreach (var text in RenderableText)
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
            RenderableText.Add("# " + _namespace.Key);
            foreach (var _class in _namespace)
            {
                MakeMarkdownFromClass(_class);
            }

            StringBuilder sb = new StringBuilder();
            foreach (var text in RenderableText)
            {
                sb.AppendLine(text);
            }

            string package = sb.ToString();
            string className = _namespace.Key;
            if (className.Contains("."))
                className = className.Split(".").Last();

            File.WriteAllText($"Markdown/Wiki/{ModuleName}/{className}.md", package);
            RenderableText = new List<string>();
        }

        if (!namespaceBound.Any())
            return "Error";
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

        RenderableText.Add($"## {className}");
        ResolveFields(_class);

        ResolveProperties(_class);

        ResolveMethods(_class);
    }

    private void ResolveMethods(Class _class)
    {
        if (_class.Methods.Any())
        {
            if (_class.Properties.Any() || _class.Fields.Any())
                RenderableText.Add("");
            //Do the same for methods
            RenderableText.Add($"### Methods:");
            RenderableText.Add("| Name  | Parameters | Return Type |");
            RenderableText.Add("| ---  | --- | --- |");
            foreach (var method in _class.Methods)
            {
                string text =
                    $"| {method.Name.Replace("<", "")} | {string.Join(", ", method.Parameters.Select(p => $"``{CSharpInternalTypes.RecursiveTypeChecker(p.ParameterType)}`` {p.Name}"))} | {CSharpInternalTypes.RecursiveTypeChecker(method.ReturnType)} |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                RenderableText.Add(text);
            }
        }
    }

    private void ResolveProperties(Class _class)
    {
        if (_class.Properties.Any())
        {
            if (_class.Fields.Any())
                RenderableText.Add("");
            RenderableText.Add($"### Properties:");
            RenderableText.Add("| Name | Type |");
            RenderableText.Add("| --- | --- |");
            foreach (var property in _class.Properties)
            {
                string text =
                    $"| {property.Name.Replace("<", "")} {StringHelpers.PropertyGetSetToString(property).Replace("{", "").Replace("}", "").Replace(";", "")} | ``{property.Type}`` |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                RenderableText.Add(text);
            }
        }
    }

    private void ResolveFields(Class _class)
    {
        if (_class.Fields.Any())
        {
            RenderableText.Add($"### Fields:");
            RenderableText.Add("| Name | Type |");
            RenderableText.Add("| --- | --- |");
            foreach (var field in _class.Fields)
            {
                string text =
                    $"| {field.Name.Replace("<", "")} | ``{CSharpInternalTypes.RecursiveTypeChecker(field.TypeDefinition)}`` |";
                text = StringHelpers.EscapeExcept(text, "|.<>[] ");
                text = removeUnderscores(text);
                RenderableText.Add(text);
            }
        }
    }

    private string removeUnderscores(string text)
    {
        return text.Replace("_", "\\_");
    }
}