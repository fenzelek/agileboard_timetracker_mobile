using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TimeTrackerXamarin._Domains.API.dto;

namespace TimeTrackerXamarin._UseCases.Contracts
{
    public class ProjectUser
    {
        [PrimaryKey]
        public int id { get; set; }
        public int user_id { get; set; }
        public int project_id { get; set; }
        [Ignore]
        public JSONDataDto<User> user { get; set; }
        public string userDB { get; set; }
    }
}
