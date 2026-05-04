namespace Business.Business;

internal class TaskEntity
{
    public virtual int Id { get; set; }
    public virtual bool IsCompleted { get; set; }
    public virtual string Title { get; set; }
    public virtual DateTime? Deadline { get; set; }
    public virtual string? Description { get; set; }
}