using UnityEngine;

namespace Noo.DevToolkit
{
    public sealed class DevConsoleHotkeys : MonoBehaviour
    {
        public void Update()
        {
            DevConsole.ProcessHotkeys();
        }
    }
}
