using System;
using System.Collections.Generic;
using System.Text;

namespace TimeTrackerXamarin._Domains.API.dto
{
    public class JSONErrorDto
    {
        public string code { get; set; }

        public double exec_time { get; set; }
    }
}
