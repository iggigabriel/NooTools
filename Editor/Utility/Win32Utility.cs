using System.ComponentModel;
using System.Diagnostics;

namespace Noo.Tools.Editor
{
    public static class Win32Utility 
    {
        public static Process RunCmd(string app, string cmd)
        {
            try
            {
                return Process.Start(app, cmd);
            }
            catch (Win32Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
            }

            return default;
        }

        public static Process ExecuteCommand(string command, bool waitForExit = false)
        {
            UnityEngine.Debug.Log($"Command: {command}");

            var processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(processInfo);

            process.OutputDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);
            process.ErrorDataReceived += (s, e) => UnityEngine.Debug.Log(e.Data);

            if (waitForExit ) process.WaitForExit();

            return process;
        }

        public static void ShowInExplorer(string folder)
        {
            folder = folder.Replace(@"/", @"\");
            Process.Start("explorer.exe", "/select," + folder);
        }
    }
}
