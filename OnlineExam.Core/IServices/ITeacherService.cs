using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Teacher;

namespace OnlineExam.Core.IServices
{
    public interface ITeacherService
    {
        Task AddTeacher(CreateTeacherDto teacherDto);
        Task<PaginatedResponse<TeacherViewModel>> GetTeachers(int pageNumber = 1, int pageSize = 10);
    }
}
