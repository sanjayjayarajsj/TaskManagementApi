
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Services
{
    public class TaskService : ITaskService
    {
        private static int nextId = 1;
        private static List<TaskItem> tasks = new();

        // CREATE TASK
        public TaskItem CreateTask(TaskDto request, int userId)
        {
            var task = new TaskItem
            {
                Id = nextId++,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                UserId = userId
            };

            tasks.Add(task);

            return task;
        }

        // GET ALL TASKS
        public List<TaskItem> GetAllTasks(
            int userId, bool isAdmin, string? search)
        {
            var result = isAdmin
                ? tasks
                : tasks.Where(t => t.UserId == userId).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                result = result
                    .Where(t => t.Title.Contains(
                        search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return result;
        }

        // GET TASK BY ID
        public TaskItem? GetTaskById(int id, int userId, bool isAdmin)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return null;
            }

            if (!isAdmin && task.UserId != userId)
            {
                return null;
            }

            return task;
        }

        // UPDATE TASK
        public TaskItem? UpdateTask(
            int id,
            TaskDto request,
            int userId,
            bool isAdmin)
        {
            var task = GetTaskById(id, userId, isAdmin);

            if (task == null)
            {
                return null;
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.Status = request.Status;

            return task;
        }

        // DELETE TASK
public bool DeleteTask(int id,int userId,bool isAdmin)
        {
            var task = GetTaskById(id, userId, isAdmin);

            if (task == null) 
            return false;
            tasks.Remove(task);
            return true;
        }
    }
}
