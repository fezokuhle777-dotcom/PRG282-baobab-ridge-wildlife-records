using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaobabRidge
{
    public class SummaryReport
    {
        public static readonly string[] StatusOrder = { "Critical", "Serious", "Stable", "Recovering", "Release-Ready" };

        public int Total { get; private set; }
        public double AverageAge { get; private set; }
        public double AverageScore { get; private set; }
        public Dictionary<string, int> StatusCounts { get; } = new Dictionary<string, int>();

        // Averages as text: two decimals with a full stop whatever the regional settings, or N/A if empty.
        public string AverageAgeText => Total == 0 ? "N/A" : AverageAge.ToString("F2", CultureInfo.InvariantCulture);
        public string AverageScoreText => Total == 0 ? "N/A" : AverageScore.ToString("F2", CultureInfo.InvariantCulture);

        // Calculates all report figures from the animals currently loaded.
        public static SummaryReport Create(List<Animal> animals)
        {
            var report = new SummaryReport { Total = animals.Count };
            foreach (string status in StatusOrder) report.StatusCounts[status] = 0;   // all five always listed

            if (animals.Count > 0)   // guards against dividing by zero
            {
                report.AverageAge = animals.Average(a => a.Age);
                report.AverageScore = animals.Average(a => a.RecoveryScore);
            }
            foreach (Animal a in animals)
                if (report.StatusCounts.ContainsKey(a.Status)) report.StatusCounts[a.Status]++;
            return report;
        }
    }

    // Builds the text for summary.txt in the layout required by Section 5.2.
        public string ToReportText(DateTime generatedAt)
        {
            string rule = new string('-', 40);
            var sb = new StringBuilder();
            sb.AppendLine("BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE");
            sb.AppendLine("ANIMAL SUMMARY REPORT");
            sb.AppendLine("Generated: " + generatedAt.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture));
            sb.AppendLine(rule);
            sb.AppendLine("Total animals:".PadRight(26) + Total);
            sb.AppendLine("Average age (years):".PadRight(26) + AverageAgeText);
            sb.AppendLine("Average recovery score:".PadRight(26) + AverageScoreText);
            sb.AppendLine(rule);
            sb.AppendLine("Animals per status:");
            foreach (string status in StatusOrder)
                sb.AppendLine(("  " + status + ":").PadRight(26) + StatusCounts[status]);
            return sb.ToString();
        }
    }
