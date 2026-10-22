using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health_Care.Models
{
    public class Admin
    {

        
            public int AdminId { get; set; }

            public string FullName { get; set; }

            public string Email { get; set; }

            public string Password { get; set; }

            public string MobileNumber { get; set; }

    }
}