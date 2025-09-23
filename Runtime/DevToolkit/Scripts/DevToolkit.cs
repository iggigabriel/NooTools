namespace Noo.DevToolkit
{
    public static class DevToolkit
    {
        static DevToolkitCommands commandsPage;
        public static DevToolkitCommands Commands => commandsPage ??= new();
    }
}
