public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed
}

public class TaskModel
{
    public int Id { get; set; }

    public string TaskID { get; set; }
    public string Task { get; set; }
    public string Assignee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted; // Use TaskStatus enum
    public string BillingType { get; set; }
    public string EmpId { get; set; }
}
