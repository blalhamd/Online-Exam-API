using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        [HttpGet]
        [HasPermission(Permissions.Users.ViewStudents)]
        public async Task<PaginatedResponse<StudentViewModel>> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _studentService.GetStudents(pageNumber, pageSize);
        }

        [HttpPost]
        [HasPermission(Permissions.Users.AddStudent)]
        public async Task CreateStudent(CreateStudentDto studentDto)
        {
            await _studentService.AddStudent(studentDto);
        }
    }
}
