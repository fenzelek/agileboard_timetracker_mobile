using System.Collections.Generic;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class DatabaseDump
    {
        public List<Table> Tables { get; set; }

        public class Table
        {
            public string Name { get; set; }
            public List<Dictionary<string, string>> Data { get; set; }
        }
    }
}