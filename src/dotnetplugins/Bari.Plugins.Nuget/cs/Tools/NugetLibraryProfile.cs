namespace Bari.Plugins.Nuget.Tools
{
    /// <summary>
    /// See https://docs.microsoft.com/en-us/nuget/reference/target-frameworks
    /// </summary>
    public enum NugetLibraryProfile
    {
        Net48 = 15,
        Net472 = 14,
        Net471 = 13,
        Net47 = 12,
        Net462 = 11,
        Net461 = 10,
        Net46 = 9,
        Net452 = 8,
        Net451 = 7,
        Net45 = 6,
        Net4 = 5,
        Net4Client = 4,
        Net35 = 3,
        Net35Client = 2,
        Net3 = 1,
        Net2 = 0
    }
}