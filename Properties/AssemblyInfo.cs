using MelonLoader;
using System.Reflection;
using Main = ChartManager.Main;

[assembly: MelonInfo(typeof(Main), Main.ModName, Main.Version, Main.Author)]
[assembly: MelonGame("PeroPeroGames", "MuseDash")]
[assembly: MelonColor(255, 255, 0, 127)]

[assembly: AssemblyTitle(Main.ModName)]
[assembly: AssemblyDescription(Main.ModDescription)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Muse Dash Modding Community")]
[assembly: AssemblyProduct(Main.ModName)]
[assembly: AssemblyCopyright($"Copyright © {Main.Author} 2026")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: AssemblyVersion(Main.Version + ".0")]
[assembly: AssemblyFileVersion(Main.Version + ".0")]