namespace Contract.Contract;

public class SaveTaskDto
{
    public bool IsCompleted { get; init; }
    public required string Title { get; init; }
    public required DateTime? Deadline { get; init; }
    public string? Description { get; init; }
}