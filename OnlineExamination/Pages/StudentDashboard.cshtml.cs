using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OnlineExamination.Pages
{
    public class StudentDashboardModel : PageModel
    {
        public string? Username { get; set; }
        public string? Role { get; set; }
        public void OnGet()
        {
            Username = HttpContext.Session.GetString("username");

            if (string.IsNullOrEmpty(Username))
            {
                // Redirect to SignIn page if session value is missing
                Response.Redirect("/Login");
                return;
            }

            Role = HttpContext.Session.GetString("role");

            if (string.IsNullOrEmpty(Role))
            {
                Response.Redirect("/Login");
                return;
            }
        }
    }
}
