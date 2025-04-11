using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
    public class ManageUserModel : PageModel
    {
        string connString = "Data Source=Taufic\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";
        public List<Users> users = new List<Users>();

        public string? errormessage { get; set; }
        public void OnGet()
        {
            loadUsers();
        }

        public void loadUsers() 
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT UserId, Username, Email, Role, IsActive FROM Users";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Users user = new Users();
                                user.UserId = reader.GetInt32(0);
                                user.Username = reader.GetString(1);
                                user.Email = reader.GetString(2);
                                user.Role = reader.GetString(3);
                                user.IsActive = reader.GetBoolean(4);
                                
                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errormessage = "Error: " + ex.Message;
            }
        }
    }
}
