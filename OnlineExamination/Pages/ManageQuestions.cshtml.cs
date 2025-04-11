using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace OnlineExamination.Pages
{
    public class ManageQuestionsModel : PageModel
    {
        string connString = "Data Source=Taufic\\SQLEXPRESS;Initial Catalog=OnlineExamination;Integrated Security=True;TrustServerCertificate=True";
        public List<Question> questions = new List<Question>();

        string errormessage = "";
        public void OnGet()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT QuestionId, ExamId, QuestionText, OptionA, OptionB, OptionC,OptionD, CorrectOption,Marks FROM Questions";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Question question = new Question();
                                question.QuestionId = reader.GetInt32(0);
                                question.ExamId = reader.GetInt32(1);
                                question.QuestionText = reader.GetString(2);
                                question.OptionA = reader.GetString(3);
                                question.OptionB = reader.GetString(4);
                                question.OptionC = reader.GetString(5);
                                question.OptionD = reader.GetString(6);
                                question.CorrectOption = reader.GetString(7);
                                question.marks = reader.GetInt32(8);
                                questions.Add(question);
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

        public void allQuestions()
        {
            
        }
    }

}
