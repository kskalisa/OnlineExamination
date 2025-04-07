using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using BCrypt.Net;

namespace OnlineExamination.Pages
{
    public class SignupModel : PageModel
    {
        string connString = "Data Source=DESKTOP-PLETT3Q\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";

        public Users theUsers = new Users();

        // --variable to hold Output Messages
        public string Message { get; set; } = "";

        // --variable to hold the Color of the Messages
        public string MessageColor { get; set; } = "black";

        public void OnGet()
        {
        }

        public void OnPost()
        {
            SaveUser();
        }

        public void SaveUser()
        {
            // -- validations

            if (string.IsNullOrEmpty(Request.Form["Email"]) ||
                string.IsNullOrEmpty(Request.Form["Username"]) ||
                string.IsNullOrEmpty(Request.Form["PasswordHash"]))
            {
                Message = "All Fields are required";
                MessageColor = "danger";

                // --return page
                return;
            }
            // Get values from the form
            theUsers.Username = Request.Form["Username"];
            theUsers.Email = Request.Form["Email"];
            string? plainPassword = Request.Form["PasswordHash"];

            // Hash the password
            theUsers.PasswordHash = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Generate OTP
            string otp = new Random().Next(100000, 999999).ToString(); // Random 6-digit OTP

            // -- save the data

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Users
							(Username, Email, PasswordHash, IsActive) VALUES
							(@_2, @_3, @_4,@_6)", conn))
                    {
                        cmd.Parameters.AddWithValue("@_2", theUsers.Username);
                        cmd.Parameters.AddWithValue("@_3", theUsers.Email);
                        cmd.Parameters.AddWithValue("@_4", theUsers.PasswordHash);
                        cmd.Parameters.AddWithValue("@_6", false);

                        conn.Open();

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Response.Redirect("Index");
                        }


                        conn.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                MessageColor = "danger";
            }
        }


    }

    public class Users
    {
        public int UserId;
        public string? Username;
        public string? PasswordHash;
        public string? Email;
        public string? Role;
        public bool IsActive;
    }
}
