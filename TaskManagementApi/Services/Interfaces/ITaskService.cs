
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;

namespace TaskManagementApi.Services.Interfaces
{
    public interface ITaskService
    {
        TaskItem CreateTask(TaskDto request, int userId);

        List<TaskItem> GetAllTasks(int userId, bool isAdmin, string? search);

        TaskItem? GetTaskById(int id, int userId, bool isAdmin);

        TaskItem? UpdateTask(int id, TaskDto request, int userId, bool isAdmin);

        bool DeleteTask(int id,int userID,bool isAdmin);
    }
}
