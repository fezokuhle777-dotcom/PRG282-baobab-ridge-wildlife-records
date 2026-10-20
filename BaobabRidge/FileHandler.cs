using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        // Reads animals.txt and returns every valid record. Malformed lines are skipped
        // and counted. Returns null (with a message) if the file cannot be read.
        public static List<Animal> LoadAnimals(out int skipped, out string errorMessage)
        {
            skipped = 0;
            errorMessage = "";
            var animals = new List<Animal>();

            try
            {
                if (!File.Exists(AnimalsPath)) return animals;   // a missing file means no animals, not an error

                foreach (string line in File.ReadAllLines(AnimalsPath))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;   // blank lines are ignored, not counted as skipped

                    string[] parts = line.Split('|');
                    int age, score;

                    // Must have 7 fields, a valid ID and numeric age and score.
                    if (parts.Length != 7
                        || !Regex.IsMatch(parts[0].Trim(), "^WR-[0-9]{4}$")
                        || !int.TryParse(parts[3].Trim(), out age)
                        || !int.TryParse(parts[4].Trim(), out score))
                    {
                        skipped++;
                        continue;
                    }

                    animals.Add(new Animal
                    {
                        AnimalID = parts[0].Trim(),
                        Name = parts[1].Trim(),
                        Species = parts[2].Trim(),
                        Age = age,
                        RecoveryScore = score,
                        Status = parts[5].Trim(),
                        HousingUnit = parts[6].Trim()
                    });
                }
                return animals;
            }
            catch (UnauthorizedAccessException)
            { errorMessage = "The animals file could not be read: permission denied."; }
            catch (IOException)
            { errorMessage = "The animals file is in use by another program. Close it and try again."; }
            return null;
        }
    }
}
