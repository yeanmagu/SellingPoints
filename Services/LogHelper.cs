using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Arkix.Modules.SellingPoints.Services
{
    public class LogHelper
    {
        public static void LOG(string log)
        {
            var root = System.Web.Hosting.HostingEnvironment.MapPath("~/DesktopModules/LOG");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            string fecha = DateTime.Now.ToString();
            String filename = root + @"\\WS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
            String cadena = fecha + " " + log + "\r\n";
            System.Console.Write(cadena);
            using (StreamWriter outfile = new StreamWriter(@filename, true))
            {
                outfile.Write(cadena);
            }
        }
    }
}