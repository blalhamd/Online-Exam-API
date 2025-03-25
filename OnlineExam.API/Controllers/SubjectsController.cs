using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Subject;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        [HasPermission(Permissions.Subjects.View)]
        public async Task<PaginatedResponse<SubjectViewModel>> GetSubjectsAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _subjectService.GetSubjectsAsync(pageNumber, pageSize);
        }
    }
}
