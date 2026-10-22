using Health_Care.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Health_Care.Controllers
{
    public class DoctorController : Controller
    {
        string path =
            "Data Source=Rushi\\MSSQLSERVER01;Initial Catalog=Health_Care;Integrated Security=True";

        // DOCTOR INDEX
      
        public ActionResult Index()
        {
            return View();
        }
        // DOCTOR REGISTRATION - GET
       
        public ActionResult DoctorRegistration()
        {
            return View();
        }


        // DOCTOR REGISTRATION - POST

        [HttpPost]
        public ActionResult DoctorRegistration(Doctor d)
        {
            SqlConnection con =
                new SqlConnection(path);

            con.Open();


           
            // CHECK EXISTING EMAIL OR MOBILE
          
            string checkQuery =
                "select Email, MobileNumber " +
                "from doctor " +
                "where Email='" +
                d.Email +
                "' or MobileNumber='" +
                d.MobileNumber +
                "'";


            SqlCommand checkCmd =
                new SqlCommand(
                    checkQuery,
                    con);


            SqlDataReader sr =
                checkCmd.ExecuteReader();


            if (sr.Read())
            {
                string existingEmail =
                    sr["Email"].ToString();

                string existingMobile =
                    sr["MobileNumber"].ToString();


                sr.Close();
                con.Close();


                // EMAIL EXISTS

                if (existingEmail == d.Email)
                {
                    ViewBag.Message =
                        "Email already exists. Please use another email.";

                    return View(d);
                }


                // MOBILE EXISTS

                if (existingMobile == d.MobileNumber)
                {
                    ViewBag.Message =
                        "Mobile number already exists. Please use another mobile number.";

                    return View(d);
                }
            }


            sr.Close();


            
            // INSERT NEW DOCTOR
            

            string query =
                "insert into doctor " +
                "(FirstName, LastName, Gender, Specialization, " +
                "Department, Experience, Qualification, MobileNumber, " +
                "Email, Address, Password) values('" +

                d.FirstName + "','" +
                d.LastName + "','" +
                d.Gender + "','" +
                d.Specialization + "','" +
                d.Department + "','" +
                d.Experience + "','" +
                d.Qualification + "','" +
                d.MobileNumber + "','" +
                d.Email + "','" +
                d.Address + "','" +
                d.Password + "')";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);


            cmd.ExecuteNonQuery();


            con.Close();


            return RedirectToAction(
                "DoctorLogin",
                "Doctor");
        }

        // DOCTOR LOGIN - GET

        public ActionResult DoctorLogin()
        {
            return View();
        }


        // DOCTOR LOGIN - POST
      
        [HttpPost]
        public ActionResult DoctorLogin(Doctor d)
        {
            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select * from Doctor " +
                "where Email='" +
                d.Email +
                "' and Password='" +
                d.Password + "'";

            SqlCommand cmd =
                new SqlCommand(query, con);

            SqlDataReader sr =
                cmd.ExecuteReader();

            if (sr.Read())
            {
                Session["DoctorId"] =
                    sr["DoctorId"].ToString();

                Session["DoctorFN"] =
                    sr["FirstName"].ToString();

                Session["DoctorLN"] =
                    sr["LastName"].ToString();

                Session["DoctorEmail"] =
                    sr["Email"].ToString();

                Session["Gender"] =
                    sr["Gender"].ToString();

                Session["Specialization"] =
                    sr["Specialization"].ToString();

                Session["Department"] =
                    sr["Department"].ToString();

                Session["Experience"] =
                    sr["Experience"].ToString();

                Session["Qualification"] =
                    sr["Qualification"].ToString();

                Session["MobileNumber"] =
                    sr["MobileNumber"].ToString();

                Session["Address"] =
                    sr["Address"].ToString();

                sr.Close();
                con.Close();

                return RedirectToAction(
                    "Doctordashboard",
                    "Doctor");
            }
            else
            {
                ViewBag.Message =
                    "Invalid email or password";

                sr.Close();
                con.Close();

                return View(d);
            }
        }


       
        // DOCTOR DASHBOARD
        
        public ActionResult Doctordashboard()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            // TODAY APPOINTMENTS

            string todayQuery =
                "select count(*) from Appointment " +
                "where DoctorId='" +
                doctorId +
                "' and AppointmentDate=CAST(GETDATE() AS DATE) " +
                "and Status<>'Cancelled'";

            SqlCommand todayCmd =
                new SqlCommand(
                    todayQuery,
                    con);

            int todayAppointments =
                Convert.ToInt32(
                    todayCmd.ExecuteScalar());


            // TOTAL PATIENTS

            string patientQuery =
                "select count(distinct Email) " +
                "from Appointment " +
                "where DoctorId='" +
                doctorId + "'";

            SqlCommand patientCmd =
                new SqlCommand(
                    patientQuery,
                    con);

            int totalPatients =
                Convert.ToInt32(
                    patientCmd.ExecuteScalar());


            // PENDING

            string pendingQuery =
                "select count(*) from Appointment " +
                "where DoctorId='" +
                doctorId +
                "' and Status='Pending'";

            SqlCommand pendingCmd =
                new SqlCommand(
                    pendingQuery,
                    con);

            int pendingAppointments =
                Convert.ToInt32(
                    pendingCmd.ExecuteScalar());


            // COMPLETED

            string completedQuery =
                "select count(*) from Appointment " +
                "where DoctorId='" +
                doctorId +
                "' and Status='Completed'";

            SqlCommand completedCmd =
                new SqlCommand(
                    completedQuery,
                    con);

            int completedAppointments =
                Convert.ToInt32(
                    completedCmd.ExecuteScalar());


            // TODAY APPOINTMENTS LIST

            string appointmentQuery =
                "select * from Appointment " +
                "where DoctorId='" +
                doctorId +
                "' and AppointmentDate=CAST(GETDATE() AS DATE) " +
                "order by AppointmentTime";

            SqlCommand appointmentCmd =
                new SqlCommand(
                    appointmentQuery,
                    con);

            SqlDataReader sr =
                appointmentCmd.ExecuteReader();

            List<Appointment> appointments =
                new List<Appointment>();

            while (sr.Read())
            {
                Appointment a =
                    new Appointment();

                a.AppointmentId =
                    Convert.ToInt32(
                        sr["AppointmentId"]);

                a.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]);

                a.FirstName =
                    sr["FirstName"].ToString();

                a.LastName =
                    sr["LastName"].ToString();

                a.Email =
                    sr["Email"].ToString();

                a.AppointmentDate =
                    Convert.ToDateTime(
                        sr["AppointmentDate"]);

                a.AppointmentTime =
                    sr["AppointmentTime"].ToString();

                a.Reason =
                    sr["Reason"].ToString();

                a.Status =
                    sr["Status"].ToString();

                if (sr["CancelReason"] != DBNull.Value)
                {
                    a.CancelReason =
                        sr["CancelReason"].ToString();
                }

                appointments.Add(a);
            }

            sr.Close();
            con.Close();


            ViewBag.TodayAppointments =
                todayAppointments;

            ViewBag.TotalPatients =
                totalPatients;

            ViewBag.PendingAppointments =
                pendingAppointments;

            ViewBag.CompletedAppointments =
                completedAppointments;

            ViewBag.Appointments =
                appointments;


            return View();
        }


       
        // LOGOUT
        
        public ActionResult Logout()
        {
            Session.Clear();

            return RedirectToAction(
                "DoctorLogin",
                "Doctor");
        }


        // MY PROFILE
       
        public ActionResult MyProfile()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select * from Doctor " +
                "where DoctorId='" +
                doctorId + "'";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            SqlDataReader sr =
                cmd.ExecuteReader();

            Doctor d =
                new Doctor();

            if (sr.Read())
            {
                d.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]);

                d.FirstName =
                    sr["FirstName"].ToString();

                d.LastName =
                    sr["LastName"].ToString();

                d.Gender =
                    sr["Gender"].ToString();

                d.Specialization =
                    sr["Specialization"].ToString();

                d.Department =
                    sr["Department"].ToString();

                d.Experience =
                    Convert.ToInt32(
                        sr["Experience"]);

                d.Qualification =
                    sr["Qualification"].ToString();

                d.MobileNumber =
                    sr["MobileNumber"].ToString();

                d.Email =
                    sr["Email"].ToString();

                d.Address =
                    sr["Address"].ToString();
            }

            sr.Close();
            con.Close();

            return View(d);
        }


        
        // UPDATE PROFILE
       
        [HttpPost]
        public ActionResult UpdateProfile(Doctor d)
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "update Doctor set " +
                "FirstName='" + d.FirstName + "'," +
                "LastName='" + d.LastName + "'," +
                "Gender='" + d.Gender + "'," +
                "Specialization='" + d.Specialization + "'," +
                "Department='" + d.Department + "'," +
                "Experience='" + d.Experience + "'," +
                "Qualification='" + d.Qualification + "'," +
                "MobileNumber='" + d.MobileNumber + "'," +
                "Email='" + d.Email + "'," +
                "Address='" + d.Address + "' " +
                "where DoctorId='" +
                doctorId + "'";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            cmd.ExecuteNonQuery();

            con.Close();


            Session["DoctorFN"] =
                d.FirstName;

            Session["DoctorLN"] =
                d.LastName;

            Session["DoctorEmail"] =
                d.Email;

            Session["Specialization"] =
                d.Specialization;

            Session["Department"] =
                d.Department;


            TempData["SuccessMessage"] =
                "Profile updated successfully!";


            return RedirectToAction(
                "MyProfile",
                "Doctor");
        }


       
        // MY PATIENTS
        
        public ActionResult MyPatients()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select distinct " +
                "p.FirstName, " +
                "p.LastName, " +
                "p.DateOfBirth, " +
                "p.Gender, " +
                "p.BloodGroup, " +
                "p.MobileNumber, " +
                "p.Email, " +
                "p.Address, " +
                "p.City " +
                "from Patients p " +
                "inner join Appointment a " +
                "on p.Email=a.Email " +
                "where a.DoctorId='" +
                doctorId + "'";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            SqlDataReader sr =
                cmd.ExecuteReader();

            List<patient> patients =
                new List<patient>();

            while (sr.Read())
            {
                patient p =
                    new patient();

                p.FirstName =
                    sr["FirstName"].ToString();

                p.LastName =
                    sr["LastName"].ToString();

                p.DateOfBirth =
                    Convert.ToDateTime(
                        sr["DateOfBirth"]);

                p.Gender =
                    sr["Gender"].ToString();

                p.BloodGroup =
                    sr["BloodGroup"].ToString();

                p.MobileNumber =
                    sr["MobileNumber"].ToString();

                p.Email =
                    sr["Email"].ToString();

                p.Address =
                    sr["Address"].ToString();

                p.City =
                    sr["City"].ToString();

                patients.Add(p);
            }

            sr.Close();
            con.Close();

            return View(patients);
        }


       
        // APPOINTMENTS
        
        public ActionResult Appointments()
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select * from Appointment " +
                "where DoctorId='" +
                doctorId +
                "' order by AppointmentDate desc";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            SqlDataReader sr =
                cmd.ExecuteReader();

            List<Appointment> appointments =
                new List<Appointment>();

            while (sr.Read())
            {
                Appointment a =
                    new Appointment();

                a.AppointmentId =
                    Convert.ToInt32(
                        sr["AppointmentId"]);

                a.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]);

                a.FirstName =
                    sr["FirstName"].ToString();

                a.LastName =
                    sr["LastName"].ToString();

                a.Email =
                    sr["Email"].ToString();

                a.AppointmentDate =
                    Convert.ToDateTime(
                        sr["AppointmentDate"]);

                a.AppointmentTime =
                    sr["AppointmentTime"].ToString();

                a.Reason =
                    sr["Reason"].ToString();

                a.Status =
                    sr["Status"].ToString();


                // CANCEL REASON

                if (sr["CancelReason"] != DBNull.Value)
                {
                    a.CancelReason =
                        sr["CancelReason"].ToString();
                }


                appointments.Add(a);
            }

            sr.Close();
            con.Close();

            return View(appointments);
        }


        
        // CONFIRM / COMPLETE APPOINTMENT
        
        [HttpPost]
        public ActionResult ManageAppointment(
            int appointmentId,
            string status)
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "update Appointment set " +
                "Status='" +
                status +
                "' " +
                "where AppointmentId='" +
                appointmentId +
                "' and DoctorId='" +
                doctorId + "'";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction(
                "Appointments",
                "Doctor");
        }


        
        // CANCEL APPOINTMENT WITH REASON
        
        [HttpPost]
        public ActionResult CancelAppointment(
            int appointmentId,
            string cancelReason)
        {
            if (Session["DoctorId"] == null)
            {
                return RedirectToAction(
                    "DoctorLogin",
                    "Doctor");
            }

            int doctorId =
                Convert.ToInt32(
                    Session["DoctorId"]);


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "update Appointment set " +
                "Status='Cancelled', " +
                "CancelReason='" +
                cancelReason +
                "' " +
                "where AppointmentId='" +
                appointmentId +
                "' and DoctorId='" +
                doctorId + "'";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);

            cmd.ExecuteNonQuery();

            con.Close();


            return RedirectToAction(
                "Appointments",
                "Doctor");
        }
    }
}