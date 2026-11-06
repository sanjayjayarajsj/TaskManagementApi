
using TaskManagementApi.DTOs;
using TaskManagementApi.Models;
using TaskManagementApi.Services.Interfaces;

namespace TaskManagementApi.Services
{
    public class TaskService : ITaskService
    {
        private static List<TaskItem> tasks = new();

        // CREATE TASK
        public TaskItem CreateTask(TaskDto request, int userId)
        {
            var task = new TaskItem
            {
                Id = tasks.Count + 1,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                UserId = userId
            };

            tasks.Add(task);

            return task;
        }

        // GET ALL TASKS
        public List<TaskItem> GetAllTasks(int userId, bool isAdmin)
        {
            if (isAdmin)
            {
                return tasks;
            }

            return tasks.Where(t => t.UserId == userId).ToList();
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
        public bool DeleteTask(int id)
        {
            var task = tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return false;
            }

            tasks.Remove(task);

            return true;
        }
    }
}
