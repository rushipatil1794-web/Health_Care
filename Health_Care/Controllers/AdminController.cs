using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;
using Health_Care.Models;

namespace Health_Care.Controllers
{
    public class AdminController : Controller
    {
        string path =
            "Data Source=Rushi\\MSSQLSERVER01;Initial Catalog=Health_Care;Integrated Security=True";


        
        // ADMIN LOGIN GET
        
        public ActionResult AdminLogin()
        {
            return View();
        }


        
        // ADMIN LOGIN POST
       
        [HttpPost]
        public ActionResult AdminLogin(string Email, string Password)
        {
            SqlConnection con =
                new SqlConnection(path);

            con.Open();

            string query =
                "select * from Admin " +
                "where Email='" + Email +
                "' and Password='" + Password + "'";

            SqlCommand cmd =
                new SqlCommand(query, con);

            SqlDataReader sr =
                cmd.ExecuteReader();

            if (sr.Read())
            {
                Session["AdminId"] =
                    sr["AdminId"].ToString();

                Session["AdminName"] =
                    sr["FullName"].ToString();

                Session["AdminEmail"] =
                    sr["Email"].ToString();

                sr.Close();
                con.Close();

                return RedirectToAction(
                    "Dashboard",
                    "Admin"
                );
            }

            sr.Close();
            con.Close();

            ViewBag.Message =
                "Invalid email or password.";

            return View();
        }


        
        // DASHBOARD
        
        public ActionResult Dashboard()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }

            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            // Total Patients

            SqlCommand patientCmd =
                new SqlCommand(
                    "select count(*) from Patients",
                    con
                );

            ViewBag.TotalPatients =
                Convert.ToInt32(
                    patientCmd.ExecuteScalar()
                );


            // Total Doctors

            SqlCommand doctorCmd =
                new SqlCommand(
                    "select count(*) from Doctor",
                    con
                );

            ViewBag.TotalDoctors =
                Convert.ToInt32(
                    doctorCmd.ExecuteScalar()
                );


            // Total Appointments

            SqlCommand appointmentCmd =
                new SqlCommand(
                    "select count(*) from Appointment",
                    con
                );

            ViewBag.TotalAppointments =
                Convert.ToInt32(
                    appointmentCmd.ExecuteScalar()
                );


            // Pending

            SqlCommand pendingCmd =
                new SqlCommand(
                    "select count(*) from Appointment " +
                    "where Status='Pending'",
                    con
                );

            ViewBag.Pending =
                Convert.ToInt32(
                    pendingCmd.ExecuteScalar()
                );


            // Confirmed

            SqlCommand confirmedCmd =
                new SqlCommand(
                    "select count(*) from Appointment " +
                    "where Status='Confirmed'",
                    con
                );

            ViewBag.Confirmed =
                Convert.ToInt32(
                    confirmedCmd.ExecuteScalar()
                );


            // Completed

            SqlCommand completedCmd =
                new SqlCommand(
                    "select count(*) from Appointment " +
                    "where Status='Completed'",
                    con
                );

            ViewBag.Completed =
                Convert.ToInt32(
                    completedCmd.ExecuteScalar()
                );


            // Cancelled

            SqlCommand cancelledCmd =
                new SqlCommand(
                    "select count(*) from Appointment " +
                    "where Status='Cancelled'",
                    con
                );

            ViewBag.Cancelled =
                Convert.ToInt32(
                    cancelledCmd.ExecuteScalar()
                );


            // Recent Appointments

            string query =
                "select top 10 Appointment.*, " +
                "Doctor.FirstName as DoctorFirstName, " +
                "Doctor.LastName as DoctorLastName, " +
                "Doctor.Specialization " +
                "from Appointment " +
                "inner join Doctor " +
                "on Appointment.DoctorId = Doctor.DoctorId " +
                "order by Appointment.AppointmentId desc";

            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );

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
                        sr["AppointmentId"]
                    );


                a.FirstName =
                    sr["FirstName"].ToString();


                a.LastName =
                    sr["LastName"].ToString();


                a.Email =
                    sr["Email"].ToString();


                a.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]
                    );


                a.DoctorName =
                    sr["DoctorFirstName"].ToString()
                    + " "
                    + sr["DoctorLastName"].ToString();


                a.Specialization =
                    sr["Specialization"].ToString();


                a.AppointmentDate =
                    Convert.ToDateTime(
                        sr["AppointmentDate"]
                    );


                a.AppointmentTime =
                    sr["AppointmentTime"].ToString();


                a.Reason =
                    sr["Reason"].ToString();


                a.Status =
                    sr["Status"].ToString();


                appointments.Add(a);
            }

            sr.Close();

            con.Close();


            return View(appointments);
        }


        
        // PATIENTS
        
        public ActionResult Patients()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "select * from Patients " +
                "order by PatientId desc";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


            SqlDataReader sr =
                cmd.ExecuteReader();


            List<patient> patients =
                new List<patient>();


            while (sr.Read())
            {
                patient p =
                    new patient();


                // IMPORTANT:
                // PatientId added for Edit/Delete

                p.PatientId =
                    Convert.ToInt32(
                        sr["PatientId"]
                    );


                p.FirstName =
                    sr["FirstName"].ToString();


                p.LastName =
                    sr["LastName"].ToString();


                p.DateOfBirth =
                    Convert.ToDateTime(
                        sr["DateOfBirth"]
                    );


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


        
        // EDIT PATIENT
       
        public ActionResult EditPatient(int patientId)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "select * from Patients " +
                "where PatientId='" +
                patientId +
                "'";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


            SqlDataReader sr =
                cmd.ExecuteReader();


            patient p =
                new patient();


            if (sr.Read())
            {
                p.PatientId =
                    Convert.ToInt32(
                        sr["PatientId"]
                    );


                p.FirstName =
                    sr["FirstName"].ToString();


                p.LastName =
                    sr["LastName"].ToString();


                p.DateOfBirth =
                    Convert.ToDateTime(
                        sr["DateOfBirth"]
                    );


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


                p.Password =
                    sr["Password"].ToString();
            }


            sr.Close();

            con.Close();


            return View(p);
        }


        
        // UPDATE PATIENT
        
        [HttpPost]
        public ActionResult UpdatePatient(patient p)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string dob =
                Convert.ToDateTime(
                    p.DateOfBirth
                ).ToString("yyyy-MM-dd");


            string query =
                "update Patients set " +

                "FirstName='" +
                p.FirstName +
                "'," +

                "LastName='" +
                p.LastName +
                "'," +

                "DateOfBirth='" +
                dob +
                "'," +

                "Gender='" +
                p.Gender +
                "'," +

                "BloodGroup='" +
                p.BloodGroup +
                "'," +

                "MobileNumber='" +
                p.MobileNumber +
                "'," +

                "Email='" +
                p.Email +
                "'," +

                "Address='" +
                p.Address +
                "'," +

                "City='" +
                p.City +
                "'," +

                "Password='" +
                p.Password +
                "' " +

                "where PatientId='" +
                p.PatientId +
                "'";


            SqlCommand updateCmd =
                new SqlCommand(
                    query,
                    con
                );


            updateCmd.ExecuteNonQuery();


            con.Close();


            TempData["SuccessMessage"] =
                "Patient updated successfully!";


            return RedirectToAction(
                "Patients",
                "Admin"
            );
        }


        
        // DELETE PATIENT
       
        [HttpPost]
        public ActionResult DeletePatient(int patientId)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string deleteQuery =
                "delete from Patients " +
                "where PatientId='" +
                patientId +
                "'";


            SqlCommand deleteCmd =
                new SqlCommand(
                    deleteQuery,
                    con
                );


            deleteCmd.ExecuteNonQuery();


            con.Close();


            TempData["SuccessMessage"] =
                "Patient deleted successfully!";


            return RedirectToAction(
                "Patients",
                "Admin"
            );
        }


        
        // DOCTORS
        
        public ActionResult Doctors()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "select * from Doctor " +
                "order by DoctorId desc";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


            SqlDataReader sr =
                cmd.ExecuteReader();


            List<Doctor> doctors =
                new List<Doctor>();


            while (sr.Read())
            {
                Doctor d =
                    new Doctor();


                d.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]
                    );


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
                        sr["Experience"]
                    );


                d.Qualification =
                    sr["Qualification"].ToString();


                d.MobileNumber =
                    sr["MobileNumber"].ToString();


                d.Email =
                    sr["Email"].ToString();


                d.Address =
                    sr["Address"].ToString();


                doctors.Add(d);
            }


            sr.Close();

            con.Close();


            return View(doctors);
        }


        // =====================================================
        // DELETE DOCTOR
        // =====================================================

        [HttpPost]
        public ActionResult DeleteDoctor(int doctorId)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            // Check doctor appointments

            string checkQuery =
                "select count(*) from Appointment " +
                "where DoctorId='" +
                doctorId +
                "'";


            SqlCommand checkCmd =
                new SqlCommand(
                    checkQuery,
                    con
                );


            int appointmentCount =
                Convert.ToInt32(
                    checkCmd.ExecuteScalar()
                );


            // If appointments exist

            if (appointmentCount > 0)
            {
                con.Close();


                TempData["ErrorMessage"] =
                    "This doctor cannot be deleted because appointments are already assigned to this doctor.";


                return RedirectToAction(
                    "Doctors",
                    "Admin"
                );
            }


            // Delete doctor

            string deleteQuery =
                "delete from Doctor " +
                "where DoctorId='" +
                doctorId +
                "'";


            SqlCommand deleteCmd =
                new SqlCommand(
                    deleteQuery,
                    con
                );


            deleteCmd.ExecuteNonQuery();


            con.Close();


            TempData["SuccessMessage"] =
                "Doctor deleted successfully!";


            return RedirectToAction(
                "Doctors",
                "Admin"
            );
        }


        // APPOINTMENTS
        
        public ActionResult Appointments()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


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
                "order by Appointment.AppointmentId desc";


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


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
                        sr["AppointmentId"]
                    );


                a.FirstName =
                    sr["FirstName"].ToString();


                a.LastName =
                    sr["LastName"].ToString();


                a.Email =
                    sr["Email"].ToString();


                a.DoctorId =
                    Convert.ToInt32(
                        sr["DoctorId"]
                    );


                a.DoctorName =
                    sr["DoctorFirstName"].ToString()
                    + " "
                    + sr["DoctorLastName"].ToString();


                a.Specialization =
                    sr["Specialization"].ToString();


                a.AppointmentDate =
                    Convert.ToDateTime(
                        sr["AppointmentDate"]
                    );


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


        
        // ADMIN PROFILE
        
        public ActionResult Profile()
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            int id =
                Convert.ToInt32(
                    Session["AdminId"]
                );


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "select * from Admin " +
                "where AdminId=" +
                id;


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


            SqlDataReader sr =
                cmd.ExecuteReader();


            Admin admin =
                new Admin();


            if (sr.Read())
            {
                admin.AdminId =
                    Convert.ToInt32(
                        sr["AdminId"]
                    );


                admin.FullName =
                    sr["FullName"].ToString();


                admin.Email =
                    sr["Email"].ToString();


                admin.Password =
                    sr["Password"].ToString();


                admin.MobileNumber =
                    sr["MobileNumber"].ToString();
            }


            sr.Close();

            con.Close();


            return View(admin);
        }


      
        // UPDATE ADMIN PROFILE
       
        [HttpPost]
        public ActionResult UpdateProfile(Admin a)
        {
            if (Session["AdminId"] == null)
            {
                return RedirectToAction(
                    "AdminLogin",
                    "Admin"
                );
            }


            SqlConnection con =
                new SqlConnection(path);

            con.Open();


            string query =
                "update Admin set " +

                "FullName='" +
                a.FullName +
                "'," +

                "Email='" +
                a.Email +
                "'," +

                "MobileNumber='" +
                a.MobileNumber +
                "'" +

                " where AdminId=" +
                a.AdminId;


            SqlCommand cmd =
                new SqlCommand(
                    query,
                    con
                );


            cmd.ExecuteNonQuery();


            con.Close();


            Session["AdminName"] =
                a.FullName;


            Session["AdminEmail"] =
                a.Email;


            TempData["SuccessMessage"] =
                "Profile updated successfully!";


            return RedirectToAction(
                "Profile",
                "Admin"
            );
        }


        
        // LOGOUT
        
        public ActionResult Logout()
        {
            Session.Clear();


            return RedirectToAction(
                "AdminLogin",
                "Admin"
            );
        }
    }
}