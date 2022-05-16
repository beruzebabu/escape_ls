using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace escape_ls.Server
{
    public class Helpers
    {
        public static string GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            return assembly.GetName().Version.ToString();
        }
    }
}
