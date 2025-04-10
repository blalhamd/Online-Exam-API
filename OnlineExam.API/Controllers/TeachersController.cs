using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineExam.API.Filters.Authentication;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Core.IServices;

namespace OnlineExam.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        [HasPermission(Permissions.Users.ViewTeachers)]
        public async Task<PaginatedResponse<TeacherViewModel>> GetTeachers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            return await _teacherService.GetTeachers(pageNumber, pageSize);
        }

        [HttpPost]
        [HasPermission(Permissions.Users.AddTeacher)]
        public async Task CreateTeacher(CreateTeacherDto teacherDto)
        {
            await _teacherService.AddTeacher(teacherDto);
        }

    }
}
