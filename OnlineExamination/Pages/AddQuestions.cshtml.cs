using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
    public class AddQuestionsModel : PageModel
    {
        string connString = "Data Source=Taufic\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";
        
        public string errormessage = "";
        public string successmessage = "";

        public List<Quiz> quizzes = new List<Quiz>();

        public Question question = new Question();
        public void OnGet()
        {
            loadQuiz();
        }
        public void OnPost()
        {
            saveQuestion();
        }
        public void loadQuiz()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT ExamId, Title, SubjectId FROM Exams";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Quiz course = new Quiz();
                                course.QuizId = reader.GetInt32(0);
                                course.title = reader.GetString(1);
                                course.coursecode = reader.GetString(2);

                                quizzes.Add(course);
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

        public void saveQuestion()
        {
            question.QuestionText = Request.Form["questiontxt"].ToString().Trim();
            question.OptionA = Request.Form["optionA"].ToString().Trim();
            question.OptionB = Request.Form["optionB"].ToString().Trim();
            question.OptionC = Request.Form["optionC"].ToString().Trim();
            question.OptionD = Request.Form["optionD"].ToString().Trim();
            question.CorrectOption = Request.Form["correctoption"].ToString().Trim();

            if (string.IsNullOrEmpty(question.QuestionText) || string.IsNullOrEmpty(question.OptionA) ||
                string.IsNullOrEmpty(question.OptionB) || string.IsNullOrEmpty(question.OptionC)
                || string.IsNullOrEmpty(question.OptionD) || string.IsNullOrEmpty(question.CorrectOption))
            {
                errormessage = "All fields are required.";
                return;
            }

            string? quizid = Request.Form["quiz"];

            if (!string.IsNullOrEmpty(quizid) && int.TryParse(quizid, out int quiz))
            {
                question.ExamId = quiz;
            }
            else
            {
                errormessage = "Invalid or missing quiz value.";
                return;
            }

            string? mark = Request.Form["markstxt"];

            if (!string.IsNullOrEmpty(mark) && int.TryParse(mark, out int markstxt))
            {
                question.marks= markstxt;
            }
            else
            {
                errormessage = "Invalid or missing marks value.";
                return;
            }




            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Questions (ExamId,QuestionText,OptionA,OptionB,OptionC,OptionD,CorrectOption,Marks) VALUES (@_1, @_2, @_3,@_4,@_5,@_6,@_7,@_8)", conn))
                    {
                        cmd.Parameters.AddWithValue("@_1", question.ExamId);
                        cmd.Parameters.AddWithValue("@_2", question.QuestionText);
                        cmd.Parameters.AddWithValue("@_3", question.OptionA);
                        cmd.Parameters.AddWithValue("@_4", question.OptionB);
                        cmd.Parameters.AddWithValue("@_5", question.OptionC);
                        cmd.Parameters.AddWithValue("@_6", question.OptionD);
                        cmd.Parameters.AddWithValue("@_7", question.CorrectOption);
                        cmd.Parameters.AddWithValue("@_8", question.marks);
                        conn.Open();
                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                errormessage = "Error: " + ex.Message;
            }

            
                // -- save the data
               
        }
    }

    public class Question
    {
        public int? QuestionId { get; set; }
        public int? ExamId { get; set; }
        public string? QuestionText { get;set; }
        public string? OptionA { get; set; }
        public string? OptionB { get; set; }
        public string? OptionC { get; set; }
        public string? OptionD { get; set; }
        public string? CorrectOption { get; set; }
        public int? marks { get; set; }

    }
}
