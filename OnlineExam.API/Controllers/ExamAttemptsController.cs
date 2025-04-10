using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Score;
using OnlineExam.Core.Dtos.UserAnswer;
using OnlineExam.Core.IServices;
using System.Security.Claims;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExamAttemptsController : ControllerBase
    {
        private readonly IExamAttemptService _examAttemptService;

        public ExamAttemptsController(IExamAttemptService examAttemptService)
        {
            _examAttemptService = examAttemptService;
        }


        [HttpPost("{examId}/start-exam")]
        [HasPermission(Permissions.ExamAttempt.StartExam)]
        public async Task<int> StartExam(int examId)
        {
            string userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value!;

            return await _examAttemptService.StartExam(userId, examId);
        }

        [HttpPut("{examAttemptId}")]
        [HasPermission(Permissions.ExamAttempt.SubmitExam)]
        public async Task<ScoreViewModel> SubmitExam(int examAttemptId, List<UserAnswerDto> answers, CancellationToken cancellation = default)
        {
            return await _examAttemptService.SubmitExam(examAttemptId, answers, cancellation);
        }
    }
}
