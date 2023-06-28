using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSServiceDriver
{
    public class FileHelper
    {
        public static void write(string message)
        {
            string folder = @"C:\POSLogs\";


            bool exists = System.IO.Directory.Exists(folder);

            if (!exists)
                System.IO.Directory.CreateDirectory(folder);

            // Filename
            string fileName = "POSServiceLogs.txt";
            // Fullpath. You can direct hardcode it if you like.
            string fullPath = folder + fileName;

            if (!File.Exists(fullPath))
            {
                // Creating the same file if it doesn't exist
                using (StreamWriter sw = File.CreateText(fullPath))
                {
                    sw.WriteLine(DateTime.Now.ToString() +" - "+ message);
                }
            } else
            {
                using (StreamWriter sw = File.AppendText(fullPath))
                {
                    sw.WriteLine(DateTime.Now.ToString() + " - " + message);
                }
            }

            //List<string> lst = new List<string>();
            //lst.Add(message);

            //// Write array of strings to a file using WriteAllLines.
            //// If the file does not exists, it will create a new file.
            //// This method automatically opens the file, writes to it, and closes file
            //File.WriteAllLines(fullPath, lst);
        }

    }
}
