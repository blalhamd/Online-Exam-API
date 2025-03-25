using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Choose.Requests;
using OnlineExam.Core.Dtos.Exam.Request;
using OnlineExam.Core.Dtos.Exam.Response;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        [HasPermission(Permissions.Exams.View)]
        public async Task<PaginatedResponse<ExamViewModel>> GetExams([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1)
        {
            return await _examService.GetExams(pageNumber, pageSize);
        }

        [HttpGet("{examId}")]
        [HasPermission(Permissions.Exams.ViewById)]
        public async Task<ExamViewModel> GetExamByIdAsync(int examId)
        {
            return await _examService.GetExamByIdAsync(examId);
        }

        [HttpPost("{examId}/assign-students")]
        [HasPermission(Permissions.Exams.AssignStudents)]
        public async Task AssignStudentsToExam(int examId,[FromBody] List<int> studentIds, CancellationToken cancellation = default)
        {
            await _examService.AssignStudentsToExam(examId, studentIds, cancellation);
        }

        [HttpPost]
        [HasPermission(Permissions.Exams.Create)]
        public async Task CreateExam(CreateExamDto model, CancellationToken cancellationToken = default)
        {
            await _examService.CreateExam(model, cancellationToken);
        }

        [HttpPut("{examId}")]
        [HasPermission(Permissions.Exams.Edit)]
        public async Task EditExam(int examId, CreateExamDto model, CancellationToken cancellationToken = default)
        {
            await _examService.EditExam(examId, model, cancellationToken);
        }

        [HttpDelete("{examId}")]
        [HasPermission(Permissions.Exams.Delete)]
        public async Task DeleteExam(int examId, CancellationToken cancellationToken = default)
        {
            await _examService.DeleteExam(examId, cancellationToken);
        }

        [HttpPost("{examId}/questions/mcq")]
        [HasPermission(Permissions.Exams.CreateChooseQuestion)]
        public async Task AddChooseQuestionToExam(int examId, CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            await _examService.AddChooseQuestionToExam(examId,model,cancellation);
        }


        [HttpPut("{examId}/questions/mcq/{questionId}")]
        [HasPermission(Permissions.Exams.EditChooseQuestion)]
        public async Task UpdateChooseQuestionToExam(int examId, int questionId, CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            await _examService.UpdateChooseQuestionToExam(examId, questionId, model, cancellation);
        }

        [HttpDelete("{examId}/questions/mcq/{questionId}")]
        [HasPermission(Permissions.Exams.DeleteChooseQuestion)]
        public async Task DeleteChooseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default)
        {
            await _examService.DeleteChooseQuestionToExam(examId, questionId, cancellation);
        }

    }
}
