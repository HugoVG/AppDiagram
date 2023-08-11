using AppExplorer;
using AppExplorer.Packages;

//Load assembly from path
string[] locations = new string[]
{
    @"AppContextMaker.dll",
};

foreach (var location in locations)
{
    //Take a random Direction 
    Random random = new Random();
    int direction = random.Next(0, 4);
    Direction direction1 = (Direction)direction;
    TypeDefiner typeDefiner = new TypeDefiner();
    typeDefiner.ReadDllTypes(location, new D2Package());
}