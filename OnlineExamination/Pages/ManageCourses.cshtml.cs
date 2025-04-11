using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
   
    public class ManageCourseModel : PageModel
    {
        string connString = "Data Source=DESKTOP-PLETT3Q\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";
        public string errormessage = "";  
        public Course course = new Course();

        public List<Course> myCourse = new List<Course>();

        public void OnGet()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT SubjectId, SubjectName FROM Subjects";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Course course = new Course();
                                course.CourseId = reader.GetString(0);
                                course.CourseName = reader.GetString(1);

                                myCourse.Add(course);
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

        public void OnPost() 
        {
        addCourse();
        }

        public void addCourse()
        {             // Get values from the form
            course.CourseId = Request.Form["coursecode"];
            course.CourseName = Request.Form["CourseName"];
            // -- validations
            if (string.IsNullOrEmpty(course.CourseId) || string.IsNullOrEmpty(course.CourseName))
            {
                errormessage = "All fields are required";
                return;
            }



            // -- save the data
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Subjects (SubjectId, SubjectName) VALUES (@_1,@_2)", conn))
                    {
                        cmd.Parameters.AddWithValue("@_1", course.CourseId);
                        cmd.Parameters.AddWithValue("@_2", course.CourseName);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                errormessage = "Error" + ex.Message;
            }
        }


    }

    public class Course
    {
        public string? CourseId { get; set; }
        public string? CourseName { get; set; }
    }
}
