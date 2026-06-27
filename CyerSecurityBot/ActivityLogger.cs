using System;
using System.Collections.Generic;

namespace CyberSecurityBot
{
    public class ActivityLogger
    {
        private List<string> _log = new List<string>();

        public void Log(string action)
        {
            string entry = $"[{DateTime.Now:HH:mm}] {action}";
            _log.Add(entry);
            if (_log.Count > 50) _log.RemoveAt(0);
        }

        public string GetRecentLog(int count = 10)
        {
            if (_log.Count == 0) return "No activity logged yet.";

            string result = "📋 ACTIVITY LOG (Recent Actions)\n";
            result += "════════════════════════════════════════\n";

            int start = Math.Max(0, _log.Count - count);
            int number = 1;

            for (int i = _log.Count - 1; i >= start; i--)
            {
                result += $"{number}. {_log[i]}\n";
                number++;
            }

            if (_log.Count > count)
                result += $"\nType 'show more' to see all {_log.Count} entries.";

            return result;
        }

        public string GetFullLog()
        {
            if (_log.Count == 0) return "No activity logged yet.";

            string result = "📋 COMPLETE ACTIVITY LOG\n";
            result += "════════════════════════════════════════\n";

            int number = 1;
            for (int i = _log.Count - 1; i >= 0; i--)
            {
                result += $"{number}. {_log[i]}\n";
                number++;
            }

            return result;
        }
    }
}
