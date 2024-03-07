using System;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public interface IErrorHandler
    {

        void Handle(Exception ex);

    }
}