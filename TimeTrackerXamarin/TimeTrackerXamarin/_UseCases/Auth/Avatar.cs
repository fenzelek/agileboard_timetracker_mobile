using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerXamarin._UseCases.Contracts.Auth;

namespace TimeTrackerXamarin._UseCases.Auth
{
    public class Avatar
    {
        private readonly IAuthService authService;

        public Avatar(IAuthService authService)
        {
            this.authService = authService;
        }

        public Task<string> Save(string avatar)
        {
            return authService.SaveAvatar(avatar);
        }
    }
}
