
namespace AutoHubAPI.Controllers
{
    internal class QuestionDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public List<AnswerOptionDto> AnswerOptions { get; set; }
    }
}