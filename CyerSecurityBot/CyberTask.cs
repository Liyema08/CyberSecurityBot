using System;

namespace CyberSecurityBot
{
    public class CyberTask
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Reminder { get; set; } = "";
        public bool IsComplete { get; set; }
        public string CreatedAt { get; set; } = "";

        public CyberTask()
        {
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }
}
