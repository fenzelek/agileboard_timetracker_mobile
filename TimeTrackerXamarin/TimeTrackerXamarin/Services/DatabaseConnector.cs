using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TimeTrackerXamarin._UseCases.Contracts;

namespace TimeTrackerXamarin.Services
{
    public class DatabaseConnector : IDatabaseConnector
    {
        private readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "timetracker.db3");
        private SQLiteAsyncConnection _connection;
        public SQLiteAsyncConnection Create()
        {
            if(_connection == null)
                _connection = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite|SQLiteOpenFlags.FullMutex|SQLiteOpenFlags.Create);

            return _connection;
        }
    }
}
