using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using UnityEngine;

namespace Nootools.Editor
{
    public static class SystemUtility 
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

    }
}
