using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaobabRidge
{
    public static class AnimalValidator
    {
        // Checks every field against the validation rules. 
        public static bool TryCreateAnimal(string idInput, string nameInput, string speciesInput,
            string ageInput, string scoreInput,
            out Animal animal, out string errorField, out string errorMessage)
        {
            animal = null; errorField = ""; errorMessage = "";

            // Trim first; the ID is upper-cased before it is checked.
            string id = idInput.Trim().ToUpperInvariant();
            string name = nameInput.Trim();
            string species = speciesInput.Trim();
            string ageText = ageInput.Trim();
            string scoreText = scoreInput.Trim();

            if (!Regex.IsMatch(id, "^WR-[0-9]{4}$"))
            { errorField = "Animal ID"; errorMessage = "Animal ID must be WR- followed by exactly 4 digits (for example WR-0007)."; return false; }

            if (name.Length < 1 || name.Length > 30 || name.Contains("|"))
            { errorField = "Name"; errorMessage = "Name must be 1-30 characters and must not contain the | character."; return false; }

            if (species.Length < 1 || species.Length > 30 || species.Contains("|"))
            { errorField = "Species"; errorMessage = "Species must be 1-30 characters and must not contain the | character."; return false; }

            // NumberStyles.None rejects signs, decimals and text, not just letters.
            if (!int.TryParse(ageText, NumberStyles.None, CultureInfo.InvariantCulture, out int age) || age > 100)
            { errorField = "Age"; errorMessage = "Age must be a whole number between 0 and 100."; return false; }

            if (!int.TryParse(scoreText, NumberStyles.None, CultureInfo.InvariantCulture, out int score) || score > 100)
            { errorField = "Recovery Score"; errorMessage = "Recovery Score must be a whole number between 0 and 100."; return false; }

            AnimalClassifier.Classify(score, out string status, out string housingUnit);
            animal = new Animal
            {
                AnimalID = id,
                Name = name,
                Species = species,
                Age = age,
                RecoveryScore = score,
                Status = status,
                HousingUnit = housingUnit
            };
            return true;
        }
    }
}
