namespace Noo.DevToolkit
{
    public static class DevToolkit
    {
        static DevToolkitCommands commandsPage;
        internal static DevToolkitCommands Commands => commandsPage ??= new();
    }
}
