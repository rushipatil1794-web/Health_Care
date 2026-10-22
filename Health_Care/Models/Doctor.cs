using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health_Care.Models
{
    public class Doctor
    {
            public int DoctorId { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Gender { get; set; }

            public string Specialization { get; set; }

            public string Department { get; set; }

            public int Experience { get; set; }

            public string Qualification { get; set; }

            public string MobileNumber { get; set; }

            public string Email { get; set; }

            public string Address { get; set; }

            public string Password { get; set; }
        
    }
}