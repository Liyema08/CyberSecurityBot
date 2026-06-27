using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class TaskManager
    {
        private TaskStorageHelper _storage = new TaskStorageHelper();
        private ActivityLogger _logger = new ActivityLogger();

        public string AddTask(string title, string description, string reminder)
        {
            _storage.AddTask(title, description, reminder);
            _logger.Log($"Task added: '{title}'");
            return $"✅ Task '{title}' added successfully!";
        }

        public List<CyberTask> GetAllTasks()
        {
            return _storage.LoadTasks();
        }

        public string MarkAsComplete(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return $"❌ Task with ID {id} not found.";

            _storage.MarkAsComplete(id);
            _logger.Log($"Task marked complete: '{task.Title}'");
            return $"✅ Task '{task.Title}' marked as complete!";
        }

        public string DeleteTask(int id)
        {
            var tasks = _storage.LoadTasks();
            var task = tasks.Find(t => t.Id == id);
            if (task == null) return $"❌ Task with ID {id} not found.";

            _storage.DeleteTask(id);
            _logger.Log($"Task deleted: '{task.Title}'");
            return $"✅ Task '{task.Title}' deleted.";
        }
    }
}
