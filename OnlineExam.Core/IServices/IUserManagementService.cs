using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.Dtos.Teacher;

namespace OnlineExam.Core.IServices
{
    public interface IUserManagementService
    {
        Task AddStudent(CreateStudentDto studentDto);
        Task AddTeacher(CreateTeacherDto teacherDto);
        Task<PaginatedResponse<StudentViewModel>> GetStudents(int pageNumber = 1, int pageSize = 10);
        Task<PaginatedResponse<TeacherViewModel>> GetTeachers(int pageNumber = 1, int pageSize = 10);
    }
}
