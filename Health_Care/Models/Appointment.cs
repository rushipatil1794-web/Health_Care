using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Health_Care.Models
{
    public class Appointment
    {
            public int AppointmentId { get; set; }

            // Patient Details
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }

            // Doctor Details
            public int DoctorId { get; set; }
            public string DoctorName { get; set; }
            public string Specialization { get; set; }

            // Appointment Details
            public DateTime AppointmentDate { get; set; }
            public string AppointmentTime { get; set; }
            public string Reason { get; set; }
            public string Status { get; set; }
            public string CancelReason { get; set; }
    }
}