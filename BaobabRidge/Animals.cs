
namespace BaobabRidge
{
    public class Animal
    {
        // Unique identification for each animal
        public string AnimalID { get; set; } = "";

        // Animal's name
        public string Name { get; set; } = "";

        // Type of animal, for example Lion or Elephant
        public string Species { get; set; } = "";

        // Animal's age
        public int Age { get; set; }

        // Recovery score between 0 and 100
        public int RecoveryScore { get; set; }

        // Animal's current rehabilitation status
        public string Status { get; set; } = "";

        // Housing unit assigned to the animal
        public string HousingUnit { get; set; } = "";
    }
}
