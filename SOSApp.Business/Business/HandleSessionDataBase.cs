using NHibernate;
using SOSApp.Contract;

namespace SOSApp.Business;

internal class HandleSessionDataBase : IHandleSessionDataBase
{
    private readonly ISession _currentSession;

    public HandleSessionDataBase()
    {
        var sessionFactory1 = new SessionFactory();
        _currentSession = sessionFactory1.OpenSession();
    }

    public void ModifyDescription(int taskId, string? description)
    {
        _currentSession.Load<TaskEntity>(taskId).Description = description;
        _currentSession.Flush();
    }

    public void HandleCompletion(int taskId, bool completed)
    {
        _currentSession.Load<TaskEntity>(taskId).IsCompleted = completed;
        _currentSession.Flush();
    }

    public void ModifyName(int taskId, string name)
    {
        _currentSession.Load<TaskEntity>(taskId).Title = name;
        _currentSession.Flush();
    }

    public void ModifyDate(int taskId, DateTime? newdeadline)
    {
        _currentSession.Load<TaskEntity>(taskId).Deadline = newdeadline;
        _currentSession.Flush();
    }

    public void SaveObject(SaveTaskDto dto)
    {
        var taskEntity = new TaskEntity
        {
            Title = dto.Title,
            Deadline = dto.Deadline,
            Description = dto.Description,
            IsCompleted = dto.IsCompleted
        };
        _currentSession.Save(taskEntity);
        _currentSession.Flush();
    }

    public void DeleteObject(int entityId)
    {
        _currentSession.Delete(_currentSession.Load<TaskEntity>(entityId));
        _currentSession.Flush();
    }

    public List<TaskDto> GetAllTasks()
    {
        return _currentSession.Query<TaskEntity>()
            .Select(entity => new TaskDto
            {
                Id = entity.Id,
                Title = entity.Title,
                Deadline = entity.Deadline,
                Description = entity.Description,
                IsCompleted = entity.IsCompleted
            }).ToList();
    }
}