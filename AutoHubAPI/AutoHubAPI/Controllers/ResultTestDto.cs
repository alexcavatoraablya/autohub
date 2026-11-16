using Application.Models.Tests;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestResultsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestResultsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/TestResults/{attemptId}
        [HttpGet("{attemptId:int}")]
        public async Task<ActionResult<TestResultDto>> GetTestResult(int attemptId)
        {
            // Припускаємо, що у вас є сутність TestAttempt (спроба проходження тесту),
            // яка пов'язана з Test та UserAnswers (відповідями користувача)
            var attempt = await _context.TestAttempts
                .AsNoTracking()
                .Include(a => a.Test)
                    .ThenInclude(t => t.Questions)
                        .ThenInclude(q => q.Options)
                .Include(a => a.UserAnswers)
                .FirstOrDefaultAsync(a => a.Id == attemptId);

            if (attempt == null)
            {
                return NotFound(new { Message = "Результат тесту не знайдено." });
            }

            // Формуємо список результатів по кожному питанню
            var questionResults = attempt.Test.Questions.Select(q =>
            {
                var selectedIds = attempt.UserAnswers
                    .Where(ua => ua.QuestionId == q.Id)
                    .Select(ua => ua.OptionId)
                    .ToList();

                var correctIds = q.Options
                    .Where(o => o.IsCorrect)
                    .Select(o => o.Id)
                    .ToList();

                // Перевірка, чи збігаються обрані варіанти з правильними
                bool isCorrect = selectedIds.Count == correctIds.Count &&
                                 !selectedIds.Except(correctIds).Any();

                int earnedPoints = isCorrect ? q.Points : 0;

                return new QuestionResultDto(
                    QuestionId: q.Id,
                    Text: q.Text,
                    Points: q.Points,
                    EarnedPoints: earnedPoints,
                    IsCorrect: isCorrect,
                    SelectedOptionIds: selectedIds,
                    CorrectOptionIds: correctIds
                );
            }).ToList();

            // Підрахунок загальних показників
            int score = questionResults.Sum(q => q.EarnedPoints);
            int maxScore = questionResults.Sum(q => q.Points);
            int correctCount = questionResults.Count(q => q.IsCorrect);
            int questionsCount = questionResults.Count;
            double percent = maxScore > 0 ? Math.Round((double)score / maxScore * 100, 2) : 0;

            // Створення фінального record TestResultDto
            var resultDto = new TestResultDto(
                TestId: attempt.TestId,
                Title: attempt.Test.Title,
                Score: score,
                MaxScore: maxScore,
                Percent: percent,
                CorrectCount: correctCount,
                QuestionsCount: questionsCount,
                Questions: questionResults
            );

            return Ok(resultDto);
        }
    }
}