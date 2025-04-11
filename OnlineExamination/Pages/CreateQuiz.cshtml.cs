using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
    public class CreateQuizModel : PageModel
    {
        string connString = "Data Source=Taufic\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";
        public string errormessage = "";
        public string successmessage = "";
        public Quiz quiz = new Quiz();

        public List<Course> myCourse = new List<Course>();
        public void OnGet()
        {
            loadCourse();
        }

        public void loadCourse()
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
            saveQuiz();
        }

        public void saveQuiz()
        {
            quiz.title = Request.Form["title"];
            quiz.coursecode = Request.Form["courses"];

            // Fix for CS8600: Handle possible null values explicitly
            string? durationValue = Request.Form["duration"];
            if (!string.IsNullOrEmpty(durationValue) && int.TryParse(durationValue, out int duration))
            {
                quiz.time = duration;
            }
            else
            {
                errormessage = "Invalid or missing duration value.";
                return;
            }

            string? examDateValue = Request.Form["examdate"];
            if (!string.IsNullOrEmpty(examDateValue))
            {
                quiz.examdate = DateTime.Parse(examDateValue);
            }
            else
            {
                errormessage = "Exam date is required.";
                return;
            }

            if (string.IsNullOrEmpty(quiz.title) || string.IsNullOrEmpty(quiz.coursecode))
            {
                errormessage = "All fields are required.";
                return;
            }

            // -- save the data
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Exams (Title, SubjectId, TimeLimitMinutes,ExamDate,IsActive) VALUES (@_1, @_2, @_3,@_4,@_5)", conn))
                    {
                        cmd.Parameters.AddWithValue("@_1", quiz.title);
                        cmd.Parameters.AddWithValue("@_2", quiz.coursecode);
                        cmd.Parameters.AddWithValue("@_3", quiz.time);
                        cmd.Parameters.AddWithValue("@_4", quiz.examdate);
                        cmd.Parameters.AddWithValue("@_5",false);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        successmessage = "Successfully Created";
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                errormessage = "Error: " + ex.Message;
            }
        }
    }

    public class Quiz
    {
        public int QuizId { get; set; }
        public string? title { get; set; }
        public string? coursecode { get; set; }
        public int? time { get; set; }
        public DateTime? examdate { get; set; }
        public bool isActive { get; set; }
    }

}
