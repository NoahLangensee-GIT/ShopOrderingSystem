using FluentNHibernate.Mapping;

namespace SOSApp.Business;

internal class TaskEntityMap : ClassMap<TaskEntity>
{
    public TaskEntityMap()
    {
        Table("Task");
        Id(todoEntity => todoEntity.Id).Column("TaskId");
        Map(todoEntity => todoEntity.IsCompleted).Column("IsCompleted");
        Map(todoEntity => todoEntity.Deadline).Column("Deadline");
        Map(todoEntity => todoEntity.Title).Column("Title");
        Map(todoEntity => todoEntity.Description).Column("Description");
    }
}