using System.Text;

namespace Domain.Entities
{
    public class Checklist
    {
        public int Id { get; private set; }
        public List<Task> Tasks { get; private set; } = [];


        public override string ToString() // TODO: move UI logic out of domain

        {
            var sb = new StringBuilder();

            sb.AppendLine($"==============================");
            sb.AppendLine($"   DAILY CHECKLIST #{Id}");
            sb.AppendLine($"==============================");

            if (Tasks.Count == 0)
            {
                sb.AppendLine("  (No tasks scheduled for today)");
            }
            else
            {
                foreach (var task in Tasks)
                {
                    var checkbox = task.IsCompleted ? "[✔]" : "[x]";
                    sb.AppendLine($"  {checkbox} {task.Name}");
                }
            }

            sb.AppendLine("==============================");
            return sb.ToString();
        }
    }
}

