using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;

        public UsersController(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        [HttpPost("add-student")]
        [HasPermission(Permissions.Users.AddStudent)]
        public async Task CreateStudent(CreateStudentDto studentDto)
        {
            await _userManagementService.AddStudent(studentDto);
        }

        [HttpPost("add-teacher")]
        [HasPermission(Permissions.Users.AddTeacher)]
        public async Task CreateTeacher(CreateTeacherDto teacherDto)
        {
            await _userManagementService.AddTeacher(teacherDto);
        }

        [HttpGet("students")]
        [HasPermission(Permissions.Users.ViewStudents)]
        public async Task<PaginatedResponse<StudentViewModel>> GetStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _userManagementService.GetStudents(pageNumber, pageSize);
        }

        [HttpGet("teachers")]
        [HasPermission(Permissions.Users.ViewTeachers)]
        public async Task<PaginatedResponse<TeacherViewModel>> GetTeachers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _userManagementService.GetTeachers(pageNumber, pageSize);  
        }
    }
}
