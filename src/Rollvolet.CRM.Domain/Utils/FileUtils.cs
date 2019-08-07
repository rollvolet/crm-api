
using System.IO;

namespace Rollvolet.CRM.Domain.Utils
{
    public class FileUtils
    {
        /*
          Ensure a directory exists
          @return directory path with a trailing slash
        */
        public static string EnsureStorageDirectory(string path)
        {
            if (!path.EndsWith(Path.DirectorySeparatorChar))
                path += Path.DirectorySeparatorChar;
            Directory.CreateDirectory(path);
            return path;
        }
      }
}