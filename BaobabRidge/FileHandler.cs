using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaobabRidge
{
    public static class FileHandler
    {
        // animals.txt lives beside the .exe, so no machine-specific paths.
        private static readonly string AnimalsPath =
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");

        // Returns true if an animal with this ID is already in the file (case-insensitive).
        public static bool IdExists(string id)
        {
            try
            {
                if (!File.Exists(AnimalsPath)) return false;
                foreach (string line in File.ReadLines(AnimalsPath))
                {
                    string[] parts = line.Split('|');
                    if (parts[0].Trim().Equals(id, StringComparison.OrdinalIgnoreCase)) return true;
                }
                return false;
            }
            catch (IOException) { return false; }                  // the append will report the real problem
            catch (UnauthorizedAccessException) { return false; }
        }

        // Appends one animal as a new line, leaving existing lines untouched.
        public static bool TryAppendAnimal(Animal animal, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                // An assessor's file might not end with a newline; start on a fresh line if so.
                string prefix = "";
                if (File.Exists(AnimalsPath))
                {
                    string existing = File.ReadAllText(AnimalsPath);
                    if (existing.Length > 0 && !existing.EndsWith("\n")) prefix = Environment.NewLine;
                }
                File.AppendAllText(AnimalsPath, prefix + animal.ToFileLine() + Environment.NewLine);
                return true;
            }
            catch (UnauthorizedAccessException)
            { errorMessage = "The animals file is read-only or you do not have permission to change it."; }
            catch (DirectoryNotFoundException)
            { errorMessage = "The folder for the animals file could not be found."; }
            catch (IOException)
            { errorMessage = "The animals file is in use by another program. Close it and try again."; }
            return false;
        }

        // Overwrites summary.txt (beside the .exe) with the report text.
        public static bool TryWriteSummary(string text, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "summary.txt");
                File.WriteAllText(path, text);   // WriteAllText overwrites; it never appends
                return true;
            }
            catch (UnauthorizedAccessException) { errorMessage = "summary.txt could not be written: permission denied."; }
            catch (DirectoryNotFoundException) { errorMessage = "The folder for summary.txt could not be found."; }
            catch (IOException) { errorMessage = "summary.txt is in use by another program. Close it and try again."; }
            return false;
        }
    }
}
