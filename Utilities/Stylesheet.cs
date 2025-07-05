namespace ConsoleGodmist;

public static class tylesheet
{
    // WPF doesn't need Spectre.Console styles, so this is now a no-op
    public static Dictionary<string, object> Styles { get; } = new();
    
    public static void InitStyles()
    {
        // Styles are not needed for WPF interface
    }
}