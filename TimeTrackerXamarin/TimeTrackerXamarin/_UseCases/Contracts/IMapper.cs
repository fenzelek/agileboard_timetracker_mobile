using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IMapper<T,U>
    {
        T Map(U tomap);
        T MapDB(U tomap);
    }
}
