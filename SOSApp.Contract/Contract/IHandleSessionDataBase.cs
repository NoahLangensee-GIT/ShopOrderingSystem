namespace Contract.Contract;

public interface IHandleSessionDataBase
{
    void ModifyDescription(int taskId, string? description);

    void HandleCompletion(int taskId, bool completed);

    void ModifyName(int taskId, string name);

    void ModifyDate(int taskId, DateTime? newdeadline);

    void SaveObject(SaveTaskDto dto);

    void DeleteObject(int entityId);

    List<TaskDto> GetAllTasks();

}