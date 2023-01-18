using System.Reflection;

namespace AppContextMaker;

public class AppClass
{
    public List<MethodInfo> methods = new List<MethodInfo>();
    public List<PropertyInfo> properties = new List<PropertyInfo>();
    public List<FieldInfo> fields = new List<FieldInfo>();
    public List<EventInfo> events = new List<EventInfo>();
}