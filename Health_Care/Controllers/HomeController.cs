using Health_Care.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace Health_Care.Controllers
{
    public class HomeController : Controller
    {
        // Connection string to the database
        string path =
            "Data Source=Rushi\\MSSQLSERVER01;Initial Catalog=Health_Care;Integrated Security=True";
      
        // HOME
       
        public ActionResult Index()
        {
            return View();
        }

      
        // REGISTRATION - GET
        

        public ActionResult Registeration()
        {
            return View();
        }


        // REGISTRATION - POST

        [HttpPost]
        public ActionResult Registeration(patient p)
        {
            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            
            // CHECK EXISTING EMAIL OR MOBILE
            
            string checkQuery =
                "select Email, MobileNumber " +
                "from Patients " +
                "where Email='" +
                p.Email +
                "' or MobileNumber='" +
                p.MobileNumber +
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

                if (existingEmail == p.Email)
                {
                    ViewBag.Message =
                        "Email already exists. Please use another email.";

                    return View(p);
                }


                // MOBILE EXISTS

                if (existingMobile == p.MobileNumber)
                {
                    ViewBag.Message =
                        "Mobile number already exists. Please use another mobile number.";

                    return View(p);
                }
            }


            sr.Close();


            
            // INSERT NEW PATIENT
            
            string dob =
                Convert.ToDateTime(
                    p.DateOfBirth)
                .ToString("yyyy-MM-dd");


            string query =
                "insert into Patients values('" +
                p.FirstName + "','" +
                p.LastName + "','" +
                dob + "','" +
                p.Gender + "','" +
                p.BloodGroup + "','" +
                p.MobileNumber + "','" +
                p.Email + "','" +
                p.Address + "','" +
                p.City + "','" +
                p.Password + "')";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con);


            cmd.ExecuteNonQuery();


            con.Close();


            return RedirectToAction(
                "Loginform",
                "Home");
        }
        
        // LOGIN - GET
        
        public ActionResult Loginform()
        {
            return View();
        }


        // LOGIN - POST
        

        [HttpPost]
        public ActionResult Loginform(patient p1)
        {
            // Connection string
           
            // Create connection
            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            // Login query
            string query =
                "select * from Patients where Email='" +
                p1.Email +
                "' and Password='" +
                p1.Password + "'";


            // Create command
            SqlCommand cmd =
                new SqlCommand(query, con);


            // Execute reader
            SqlDataReader sr =
                cmd.ExecuteReader();


            // Check user
            if (sr.Read())
            {
                // Get First Name
                string FirstName =
                    sr["FirstName"].ToString();


                // Store First Name in Session
                Session["FN"] =
                    FirstName;


                // Get Last Name
                string LastName =
                    sr["LastName"].ToString();


                // Store Last Name in Session
                Session["LN"] =
                    LastName;


                // Store Email in Session
                Session["Email"] =
                    sr["Email"].ToString();


                // Close connection
                con.Close();


                // Redirect to Home page
                return RedirectToAction("PatientDashboard", "Home");
            }
            else
            {
                // Invalid login message
                ViewBag.Message =
                    "Invalid email or password";


                // Close connection
                con.Close();


                // Return login page
                return View(p1);
            }
        }
       public ActionResult BookAppointment()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Loginform", "Home");
            }

            SqlConnection con = new SqlConnection(path);
            con.Open();

            string query = "select * from doctor";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sr = cmd.ExecuteReader();

            List<Doctor> doctors = new List<Doctor>();

            while (sr.Read())
            {
                Doctor d = new Doctor();

                d.DoctorId = Convert.ToInt32(sr["DoctorId"]);
                d.FirstName = sr["FirstName"].ToString();
                d.LastName = sr["LastName"].ToString();
                d.Specialization = sr["Specialization"].ToString();
                d.Department = sr["Department"].ToString();

                doctors.Add(d);
            }

            sr.Close();
            con.Close();

            ViewBag.Doctors = doctors;

            return View();
        }
        [HttpPost]
        public ActionResult BookAppointment(Appointment a)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Loginform", "Home");
            }

            SqlConnection con = new SqlConnection(path);
            con.Open();

            string query =
                "insert into Appointment " +
                "(DoctorId, FirstName, LastName, Email, AppointmentDate, AppointmentTime, Reason) " +
                "values('" +
                a.DoctorId + "','" +
                a.FirstName + "','" +
                a.LastName + "','" +
                a.Email + "','" +
                a.AppointmentDate.ToString("yyyy-MM-dd") + "','" +
                a.AppointmentTime + "','" +
                a.Reason + "')";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();

            return RedirectToAction("PatientDashboard", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("PatientDashboard", "Home");
        }
        public ActionResult PatientDashboard()
        {
            
            // Login check
            if (Session["Email"] == null)
            {
                return RedirectToAction("Loginform", "Home");
            }

            string email = Session["Email"].ToString();

            SqlConnection con = new SqlConnection(path);
            con.Open();


            
            // TOTAL APPOINTMENTS
            
            string totalQuery =
                "select count(*) from Appointment where Email='" +
                email + "'";

            SqlCommand totalCmd =
                new SqlCommand(totalQuery, con);

            int totalAppointments =
                Convert.ToInt32(totalCmd.ExecuteScalar());


            // PENDING APPOINTMENTS
          

            string pendingQuery =
                "select count(*) from Appointment " +
                "where Email='" + email + "' " +
                "and Status='Pending'";

            SqlCommand pendingCmd =
                new SqlCommand(pendingQuery, con);

            int pendingAppointments =
                Convert.ToInt32(pendingCmd.ExecuteScalar());


            // CONFIRMED APPOINTMENTS
           

            string confirmedQuery =
                "select count(*) from Appointment " +
                "where Email='" + email + "' " +
                "and Status='Confirmed'";

            SqlCommand confirmedCmd =
                new SqlCommand(confirmedQuery, con);

            int confirmedAppointments =
                Convert.ToInt32(confirmedCmd.ExecuteScalar());


            // DOCTORS VISITED
            
            string doctorsQuery =
                "select count(distinct DoctorId) " +
                "from Appointment " +
                "where Email='" + email + "'";

            SqlCommand doctorsCmd =
                new SqlCommand(doctorsQuery, con);

            int doctorsVisited =
                Convert.ToInt32(doctorsCmd.ExecuteScalar());


            con.Close();


            
            // SEND DATA TO VIEW
            

            ViewBag.TotalAppointments =
                totalAppointments;

            ViewBag.PendingAppointments =
                pendingAppointments;

            ViewBag.ConfirmedAppointments =
                confirmedAppointments;

            ViewBag.DoctorsVisited =
                doctorsVisited;


            return View();

        }
        public ActionResult Profile()
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Loginform", "Home");
            }

            string email = Session["Email"].ToString();

            SqlConnection con = new SqlConnection(path);
            con.Open();

            string query =
                "select * from Patients where Email='" +
                email + "'";

            SqlCommand cmd = new SqlCommand(query, con);

            SqlDataReader sr = cmd.ExecuteReader();

            if (sr.Read())
            {
                ViewBag.FirstName = sr["FirstName"].ToString();
                ViewBag.LastName = sr["LastName"].ToString();
                ViewBag.Email = sr["Email"].ToString();
                ViewBag.DateOfBirth = sr["DateOfBirth"].ToString();
                ViewBag.Gender = sr["Gender"].ToString();
                ViewBag.BloodGroup = sr["BloodGroup"].ToString();
                ViewBag.MobileNumber = sr["MobileNumber"].ToString();
                ViewBag.Address = sr["Address"].ToString();
                ViewBag.City = sr["City"].ToString();
            }

            sr.Close();
            con.Close();

            return View();
        }
       


        // UPDATE PROFILE
        [HttpPost]
        public ActionResult UpdateProfile(patient p)
        {
            if (Session["Email"] == null)
            {
                return RedirectToAction("Loginform", "Home");
            }

            string oldEmail = Session["Email"].ToString();

            SqlConnection con = new SqlConnection(path);
            con.Open();

            string dob =
                Convert.ToDateTime(p.DateOfBirth)
                .ToString("yyyy-MM-dd");

            string query =
                "update Patients set " +
                "FirstName='" + p.FirstName + "'," +
                "LastName='" + p.LastName + "'," +
                "DateOfBirth='" + dob + "'," +
                "Gender='" + p.Gender + "'," +
                "BloodGroup='" + p.BloodGroup + "'," +
                "MobileNumber='" + p.MobileNumber + "'," +
                "Email='" + p.Email + "'," +
                "Address='" + p.Address + "'," +
                "City='" + p.City + "' " +
                "where Email='" + oldEmail + "'";

            SqlCommand cmd = new SqlCommand(query, con);

            cmd.ExecuteNonQuery();

            con.Close();

            // Update Session Email also
            Session["Email"] = p.Email;

            Session["FN"] = p.FirstName;
            Session["LN"] = p.LastName;
            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction("Profile", "Home");
        }
        
        
        public ActionResult History()
        {
          
            if (Session["Email"] == null)
            {
                return RedirectToAction(
                    "Loginform",
                    "Home");
            }

            string email =
                Session["Email"].ToString();

            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select Appointment.*, " +
                "Doctor.FirstName as DoctorFirstName, " +
                "Doctor.LastName as DoctorLastName, " +
                "Doctor.Specialization " +
                "from Appointment " +
                "inner join Doctor " +
                "on Appointment.DoctorId = Doctor.DoctorId " +
                "where Appointment.Email='" +
                email +
                "' " +
                "order by Appointment.AppointmentDate desc";

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


                // DOCTOR NAME

                string doctorFirstName =
                    sr["DoctorFirstName"].ToString();

                string doctorLastName =
                    sr["DoctorLastName"].ToString();

                a.DoctorName =
                    doctorFirstName +
                    " " +
                    doctorLastName;


                // SPECIALIZATION

                a.Specialization =
                    sr["Specialization"].ToString();


                // CANCEL REASON

                if (sr["CancelReason"] != DBNull.Value)
                {
                    a.CancelReason =
                        sr["CancelReason"].ToString();
                }
                else
                {
                    a.CancelReason = "";
                }


                appointments.Add(a);
            }


            sr.Close();

            con.Close();


            return View(appointments);
        
        }


    }
}