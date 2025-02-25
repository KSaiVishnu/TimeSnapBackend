public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed
}

public class TaskModel
{
    public int Id { get; set; }

    public required string TaskID { get; set; }
    public required string Task { get; set; }
    public required string Assignee { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime DueDate { get; set; }
    public TaskStatus Status { get; set; } = TaskStatus.NotStarted; // Use TaskStatus enum
    public required string BillingType { get; set; }
    public required string EmpId { get; set; }
}
