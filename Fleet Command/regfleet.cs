using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Fleet_Command
{
    static class regfleet
    {
        public static void updateregister()
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software", true);
            key = key.OpenSubKey("Classes", true);
            key = key.CreateSubKey("fleetcommand");
            key.SetValue("", "URL:Fleet Command");
            key.SetValue("URL Protocol", "");
            key = key.CreateSubKey("shell");
            key = key.CreateSubKey("open");
            key = key.CreateSubKey("command");
            string path = "\"" + System.Reflection.Assembly.GetExecutingAssembly().Location + "\" \"%1\"";
            key.SetValue("", path);

        }
    }
}
