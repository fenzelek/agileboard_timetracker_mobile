using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SQLite;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Companies;
using TimeTrackerXamarin._UseCases.Contracts.TimeTracking;
using Xamarin.Forms.Internals;

namespace TimeTrackerXamarin.Services
{
    public class DebugTools : IDebugTools
    {
        private readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "timetracker.db3");
        
        public DatabaseDump DumpDatabase()
        {
            var conn = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadOnly);
            var tables = new List<DatabaseDump.Table>
            {
                MapToTable(conn.Table<TimeFrame>().ToList(), "TimeFrame"),
                MapToTable(conn.Table<Company>().ToList(), "Company"),
                MapToTable(conn.Table<Project>().ToList(), "Project"),
                MapToTable(conn.Table<Ticket>().ToList(), "Ticket"),
                MapToTable(conn.Table<TicketDetails>().ToList(), "TicketDetails"),
                MapToTable(conn.Table<ProjectUser>().ToList(), "ProjectUser")
            };
            conn.Close();
            return new DatabaseDump
            {
                Tables = tables
            };
        }

        private static DatabaseDump.Table MapToTable<T>(IEnumerable<T> list, string tableName)
        {
            var properties = typeof(T).GetProperties()
                .Where((p) => !Attribute.IsDefined(p, typeof(IgnoreAttribute)));
            
            var data = new List<Dictionary<string, string>>();
            foreach (var value in list)
            {
                var row = new Dictionary<string, string>();
                foreach (var propertyInfo in properties)
                {
                    var propertyValue = propertyInfo.GetValue(value, null);
                    string propertyValueString = "null";
                    if (propertyValue != null)
                    {
                        propertyValueString = propertyValue.ToString();
                    }
                    row.Add(propertyInfo.Name, propertyValueString);
                }
                data.Add(row);
            }

            return new DatabaseDump.Table
            {
                Name = tableName,
                Data = data
            };
        }
    }
}