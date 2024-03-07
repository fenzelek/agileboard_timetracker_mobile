using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IDatabaseConnector
    {
        SQLiteAsyncConnection Create();
    }
}
