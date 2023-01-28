using AppExplorer;
using AppExplorer.Packages;

//Load assembly from path
// Assembly assembly = Assembly.LoadFrom(@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\HRtheGathering\\HRtheGathering\\bin\\Debug\\net6.0\\HRtheGathering.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule (@"C:\\Users\\megah\\Source\\Repos\\\BakaBot\\\BakaBotDiscord\\\bin\\Debug\\net7.0\\BakaBotDiscord.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule("C:\\Users\\megah\\Source\\Repos\\Nutrient\\bin\\Debug\\net7.0\\Nutrient.dll");
// ModuleDefinition module = ModuleDefinition.ReadModule ("AppContextMaker.dll");
string[] locations = new string[]
{
    //@"C:\\Users\\megah\\Source\\Repos\\Album-Api\\Album-Api\\bin\\Debug\\net6.0\\publish\\Album-Api.dll",
    // @"C:\\Users\\megah\\Source\\Repos\\HRtheGathering\\HRtheGathering\\bin\\Debug\\net6.0\\HRtheGathering.dll",
    @"C:\\Users\\megah\\Source\\Repos\\\BakaBot\\\BakaBotDiscord\\\bin\\Debug\\net7.0\\BakaBotDiscord.dll",
    // @"C:\\Users\\megah\\Source\\Repos\\Nutrient\\bin\\Debug\\net7.0\\Nutrient.dll",
    // @"AppContextMaker.dll",
    // "C:\\Users\\megah\\Downloads\\Concert_Overlay_Windows_1.1.0\\Concert_Overlay_Windows_1.1.0\\Concert Overlay_Data\\Managed\\Assembly-CSharp.dll"
};
// ReadDllTypes(dllLocation);

foreach (var location in locations)
{
    //Take a random Direction 
    Random random = new Random();
    int direction = random.Next(0, 4);
    Direction direction1 = (Direction)direction;
    TypeDefiner typeDefiner = new TypeDefiner();
    typeDefiner.ReadDllTypes(location, new MDPackage());
}