using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._Domains.API;
using TimeTrackerXamarin._Domains.API.dto;
using TimeTrackerXamarin._Domains.Auth.Dto;
using TimeTrackerXamarin._UseCases.Contracts;
using TimeTrackerXamarin._UseCases.Contracts.Auth;

namespace TimeTrackerXamarin._UseCases.Auth
{
    public class Login
    {
        private readonly IAuthService authService;

        public Login(IAuthService authService)
        {
            this.authService = authService;
        }

        public Task Exec(LoginDto loginDto) 
        {
            return authService.Login(loginDto); 
        }
    }
}
