using System.IO;

namespace upos_device_simulation.Helpers
{
    public class FileHelper
    {
        public  void CreateLogFile(string filePath)
        {
            if (!File.Exists(filePath))
                File.Create(filePath);
        }
    }
}
