using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BaobabRidge
{
    public static class AnimalClassifier
    {
        // Works out status and housing unit from a validated score (0-100).
        // This is the only place this logic exists; Add and Update both call it.
        public static void Classify(int score, out string status, out string housingUnit)
        {
            if (score <= 19) 
                { status = "Critical"; housingUnit = "Intensive Care Unit"; }
            else if (score <= 39) 
                { status = "Serious"; housingUnit = "High-Dependency Ward"; }
            else if (score <= 59) 
                { status = "Stable"; housingUnit = "Recovery Ward"; }
            else if (score <= 79) 
                { status = "Recovering"; housingUnit = "Outdoor Enclosure"; }
            else 
                { status = "Release-Ready"; housingUnit = "Pre-Release Camp"; }
        }
    }
    
}
