using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.Services
{
    public class FileLogStorage : ILogStorage
    {
        
        private readonly Regex logRegex = new Regex(@"^\[(.*)\] \[(\d\d\.\d\d.\d\d\d\d \d\d:\d\d:\d\d)\] (.*)$");
        private readonly string logsDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "logs");
        private readonly string dateFormat = "dd.MM.yyyy hh:mm:ss";
        
        public void Write(Log log)
        {
            var file = GetFile(DateTime.Now);
            var writer = File.AppendText(file);
            writer.Write(SerializeLog(log));
            writer.Flush();
            writer.Close();
        }

        public List<Log> GetLogs(DateTime dateTime)
        {
            var file = GetFile(dateTime);
            if (!File.Exists(file))
            {
                return new List<Log>();
            }

            var lines = File.ReadAllLines(file);
            var logs = new List<Log>();
            foreach (var line in lines)
            {
                var match = logRegex.Match(line);
                if (!match.Success)
                {
                    var last = logs.LastOrDefault();
                    if (last != null)
                    {
                        last.Message += line;
                    }
                    continue;
                }

                var log = ParseLog(match);
                if (log != null)
                {
                    logs.Add(log);   
                }
            }
            return logs;
        }

        public List<DateTime> GetSavedLogDates()
        {
            if (!Directory.Exists(logsDirectory))
            {
                return new List<DateTime>();
            }

            var files = Directory.EnumerateFiles(logsDirectory, "*.log")
                .Select(Path.GetFileNameWithoutExtension);

            var dates = new List<DateTime>();
            foreach (var file in files)
            {
                DateTime date;
                if (!DateTime.TryParseExact(file, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                {
                    continue;
                };
                
                dates.Add(date);
            }

            return dates;
        }

        string GetFile(DateTime dateTime)
        {
            var dateString = dateTime.ToString("yyyy-MM-dd");
            var fileName = $"{dateString}.log";
            if (!Directory.Exists(logsDirectory))
            {
                Directory.CreateDirectory(logsDirectory);
            }
            return Path.Combine(logsDirectory, fileName);
        }

        private string SerializeLog(Log log)
        {
            var dateString = log.Date.ToString(dateFormat);
            return $"[{log.Type}] [{dateString}] {log.Message}\n";
        }

        private Log ParseLog(Match match)
        {
            var groups = match.Groups;
            Log.LogType type;
            switch (groups[1].ToString())
            {
                case "Info":
                    type = Log.LogType.Info;
                    break;
                case "Warn":
                    type = Log.LogType.Warn;
                    break;
                case "Error":
                    type = Log.LogType.Error;
                    break;
                case "Debug":
                    type = Log.LogType.Debug;
                    break;
                default:
                    return null;
            }

            DateTimeOffset date;
            if (!DateTimeOffset.TryParse(groups[2].ToString(), out date))
            {
                return null;
            }

            var message = groups[3].ToString();
            return new Log
            {
                Date = date,
                Message = message,
                Type = type
            };
        }
    }
}