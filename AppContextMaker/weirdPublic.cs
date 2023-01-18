namespace AppContextMaker;

public class weirdPublic
{
    private int a;
    private int b { get; set; }
    public int c;
    public int d { get; set; }
    public int f { get; }
    public int g { get; init; } 
    public List<int> e { get; set; }

    public henkEnum GetName()
    {
        return henkEnum.a;
    }

    public int henks()
    {
        var i = new henkStruct();
        i.a = 5;
        return i.a;
    }

    public T Harrie<T>(T t)
    {
        return t;
    }
    private int henks2(string c)
    {
        var i = new henkStruct() ;
        c = "henk";
        i.a = 5;
        return i.a;
    }
    public weirdPublic()
    {
        a = 1;
        b = 2;
        c = 3;
        d = 4;
    }
    
}

public interface IBarrieService
{
    
}
public class BarrieService : IBarrieService
{
    
}
public interface IHarrieService
{
    
}
public class HarrieService : IHarrieService
{
    private readonly IBarrieService _barrieService;
}
public class HarrieService2 : HarrieService
{
    private readonly IBarrieService _barrieService;
    private readonly henkPrivate _henkPrivate;
}
internal class henkPrivate
{
    
    private readonly IBarrieService _barrieService;
    public int a;
    private int b;
    public weirdPublic c;

    public async Task<weirdPublic> d(IEnumerable<string> t)
    {
        return new weirdPublic();
    }
}
public enum henkEnum
{
    a,
    b,
    c
}
struct henkStruct
{
    public int a;
    private int b;
}
ref struct henkRefStruct
{
    public int a;
    private int b;
}

