using System;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class ApiErrorException : Exception
    {
        public string Code { get; }
        
        public ApiErrorException(string code) : base($"Api error {code}")
        {
            Code = code;
        }
    }
}