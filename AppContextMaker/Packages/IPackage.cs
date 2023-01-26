using Mono.Cecil;
using TypeDefinition = Mono.Cecil.TypeDefinition;

namespace AppContextMaker.Packages;

public interface IPackage
{
    /// <summary>
    /// Make an Object from <see cref="Objects"/> fields
    /// </summary>
    public void MakeClass(Class _class);
    public void MakeConnections(Objects FromObject, Objects ToObject, string text = "");
    public void MakeStringConnections(string FromObject, string ToObject, string text = "");
    public void MakeTypedConnections(TypeDefinition FromObject, TypeReference ToObject, string text = "");
    public List<Class> Classes { get; set; }
    /// <summary>
    /// For diagrams
    /// </summary>
    public List<string> Connections { get; set; }
}

public interface IDiagram : IPackage
{
    /// <summary>
    /// Ensemble the diagram
    /// </summary>
    /// <returns></returns>
    public string Package();
    /// <summary>
    /// Headers for the diagram
    /// </summary>
    /// <returns></returns>
    public void Start();
    /// <summary>
    /// For making connections and or start a process
    /// </summary>
    /// <returns></returns>
    public void End();
}

public class Objects
{
    public string Name { get; set; } // Name of the object
    public Visibility Visibility { get; set; } // Public, Private, Protected
    public string Type { get; set; } // Class, Interface, Struct, Enum
    public List<Objects> Connections { get; set; } = new List<Objects>(); // List of connections to other objects
    public TypeDefinition TypeDefinition { get; set; } // TypeDefinition from Cecil
    public void SetObject(string name, Visibility visibility, string type)
    {
        Name = name;
        Visibility = visibility;
        Type = type;
    }
}

public class Class : Objects
{
    public string Namespace { get; set; } = "";
    public ObjectType ObjectType { get; set; } = ObjectType.Class;
    public List<Property> Properties { get; set; } = new List<Property>();
    public List<Method> Methods { get; set; } = new List<Method>();
    public List<Field> Fields { get; set; } = new List<Field>();
    public List<Event> Events { get; set; } = new List<Event>();
}

public class Method : Objects
{
    public List<ParameterDefinition> Parameters { get; set; } = new List<ParameterDefinition>();
    public TypeReference ReturnType { get; set; }
}
public class Property : Objects
{
    public bool IsGet { get; set; }
    public bool IsSet { get; set; }
    public Objects ReturnType { get; set; }
}
public class Field : Objects
{
    
}
public class Event : Objects
{
    
}

public enum Visibility
{
    Public,
    Private,
    Protected,
}
public enum ObjectType
{
    Class,
    Interface,
    Struct,
    Enum,
}