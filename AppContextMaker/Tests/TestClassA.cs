namespace AppContextMaker.Tests;

public class TestClassA : TestClassAParent
{
    public string Name { get; set; }
    public new string overriden { get; set; }

    public void Say()
    {
        int a = 5;
        int b = 6;
        return;
    }
}


public class TestClassAParent
{
    public int value { get; set; }
    public string overriden { get; set; }
}