using System.Text;

namespace Domain.Entities
{
    public class Task
    {
        public int Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Metadata { get; private set; } = string.Empty;

        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        // for analyzing personal data
        // to improve productivity later
        public TimeSpan? AttemptDuration { get; private set; }
        public int EffortRating { get; private set; } // 0 - 100
        public bool IsCompleted { get; private set; }
        public bool Attempted { get; private set; }



        public override string ToString() // TODO: move UI logic out of domain
        {
            var sb = new StringBuilder();

            sb.AppendLine($"{Name} {(IsCompleted ? "[✔]" : "[x]")}");

            if (!string.IsNullOrWhiteSpace(Metadata))
                sb.AppendLine($"  Info: {Metadata}");

            sb.AppendLine($"  Start: {StartTime:HH:mm}  End: {EndTime:HH:mm}");

            if (Attempted)
            {
                if (AttemptDuration != null)
                    sb.AppendLine($"  Attempted: Yes, Duration: {AttemptDuration:c}");
                sb.AppendLine($"  Effort: {EffortRating}/100");
            }
            else
            {
                sb.AppendLine("  Attempted: No");
            }

            return sb.ToString();
        }
    }
}
