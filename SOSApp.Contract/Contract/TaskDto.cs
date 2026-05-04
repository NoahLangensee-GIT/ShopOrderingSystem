namespace SOSApp.Contract;

public class TaskDto
{
    public int Id { get; init; }
    public bool IsCompleted { get; init; }
    public required string Title { get; init; }
    public required DateTime? Deadline { get; init; }
    public string? Description { get; init; }
}