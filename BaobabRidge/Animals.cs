using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaobabRidge
{
    class Animals
    {
        public string AnimalID { get; set; } = "";
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
        public string Status { get; set; }
        public string HousingUnit { get; set; }

        // Converts the animal into one seperated line for animals.txt.
        public string ToFileLine()
        {
            return string.Join("|", AnimalID, Name, Species, Age, RecoveryScore, Status, HousingUnit);
        }
    }
}
