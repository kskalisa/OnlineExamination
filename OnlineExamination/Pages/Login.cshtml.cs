using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
    public class LoginModel : PageModel
    {
        string connString = "Data Source=Taufic\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";

        public Users theUsers = new Users();
        public string Message { get; set; } = "";
        public string MessageColor { get; set; } = "black";
        public void OnGet()
        {

        }

        public void OnPost()
        {
            RetrieveUser();
        }
        public void RetrieveUser()
        {
            if (string.IsNullOrEmpty(Request.Form["Username"]) || string.IsNullOrEmpty(Request.Form["PasswordHash"]))
            {
                Message = "All Fields are required";
                MessageColor = "danger";
                return;
            }

            string? username = Request.Form["Username"];
            string? plainPassword = Request.Form["PasswordHash"];

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM Users WHERE Username = @_2", conn))
                    {
                        cmd.Parameters.AddWithValue("@_2", username);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                theUsers.UserId = reader.GetInt32(reader.GetOrdinal("UserId"));
                                theUsers.Username = reader.GetString(reader.GetOrdinal("Username"));
                                theUsers.Email = reader.GetString(reader.GetOrdinal("Email"));
                                theUsers.PasswordHash = reader.GetString(reader.GetOrdinal("PasswordHash"));
                                theUsers.Role = reader.GetString(reader.GetOrdinal("Role"));
                                theUsers.IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));

                                if (BCrypt.Net.BCrypt.Verify(plainPassword, theUsers.PasswordHash))
                                {
                                     theUsers.IsActive = true;
                                        HttpContext.Session.SetInt32("userId", theUsers.UserId);
                                        HttpContext.Session.SetString("role", theUsers.Role);
                                        HttpContext.Session.SetString("username", theUsers.Username);

                                        if (theUsers.Role.Equals("Admin"))
                                        {
                                            Response.Redirect("/AdminDashboard");
                                        }
                                        else
                                        {

                                            Response.Redirect("/StudentDashboard");
                                        }
                                   
                                }
                                else
                                {
                                    Message = "Invalid username or password";
                                    MessageColor = "danger";
                                }

                            }

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
}
