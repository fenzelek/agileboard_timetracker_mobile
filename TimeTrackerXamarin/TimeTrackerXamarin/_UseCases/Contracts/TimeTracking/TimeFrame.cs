using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts.TimeTracking
{
    public class TimeFrame
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public long from { get; set; }
        public long to { get; set; }
        public int companyId { get; set; }
        public int projectId { get; set; }
        public int taskId { get; set; }
        public int activity { get; set; }
        public string gpsPositionDB { get; set; }
        [Ignore]
        public string[] screens { get; set; }
        [Ignore]
        public IDictionary<string, double?> gpsPosition { get; set; }
        public bool? sended { get; set; }
    }
}
