using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._Domains.Companies;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IFactory<T>
    {
        T Create(bool connection);
    }
}
