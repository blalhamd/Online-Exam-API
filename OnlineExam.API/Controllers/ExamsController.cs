namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting(RateLimiterType.Concurrency)]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [Authorize(Roles = $"{Role.Admin},{Role.User}")]
        public async Task<ActionResult<PaginatedResponse<ExamDto>>> GetExams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1)
        {
            var exams = await _examService.GetExams(pageNumber, pageSize);

            return Ok(exams);
        }

        [HttpGet("{examId}")]
        [Authorize(Roles = $"{Role.Admin},{Role.User}")]
        public async Task<ActionResult<ExamDto>> GetExamByIdAsync(int examId)
        {
            var exam = await _examService.GetExamByIdAsync(examId);

            return Ok(exam);
        }

        [HttpPost("{examId}/assign-students")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AssignStudentsToExam(int examId,[FromBody] List<int> studentIds, CancellationToken cancellation = default)
        {
            await _examService.AssignStudentsToExam(examId, studentIds, cancellation);

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> CreateExam(CreateExamDto model, CancellationToken cancellationToken = default)
        {
            await _examService.CreateExam(model, cancellationToken);

            return NoContent();
        }

        [HttpPut("{examId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> EditExam(int examId, CreateExamDto model, CancellationToken cancellationToken = default)
        {
            await _examService.EditExam(examId, model, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{examId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteExam(int examId, CancellationToken cancellationToken = default)
        {
            await _examService.DeleteExam(examId, cancellationToken);

            return NoContent();
        }

        [HttpPost("{examId}/questions/mcq")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AddChooseQuestionToExam(int examId, CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            await _examService.AddChooseQuestionToExam(examId,model,cancellation);

            return NoContent();
        }

        [HttpPost("{examId}/questions/true-false")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> AddTrueOrFalseQuestionToExam(int examId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default)
        {
            await _examService.AddTrueOrFalseQuestionToExam(examId,model, cancellation);
            
            return NoContent();
        }

        [HttpPut("{examId}/questions/mcq/{questionId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateChooseQuestionToExam(int examId, int questionId, CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            await _examService.UpdateChooseQuestionToExam(examId, questionId, model, cancellation);

            return NoContent();
        }

        [HttpPut("{examId}/questions/true-false/{questionId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> UpdateTrueOrFalseQuestionToExam(int examId, int questionId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default)
        {
            await _examService.UpdateTrueOrFalseQuestionToExam(examId, questionId, model, cancellation);

            return NoContent();
        }

        [HttpDelete("{examId}/questions/mcq/{questionId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteChooseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default)
        {
            await _examService.DeleteChooseQuestionToExam(examId, questionId, cancellation);

            return NoContent();
        }

        [HttpDelete("{examId}/questions/true-false/{questionId}")]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> DeleteTrueOrFalseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default)
        {
            await _examService.DeleteTrueOrFalseQuestionToExam(examId, questionId, cancellation);

            return NoContent();
        }


    }
}
