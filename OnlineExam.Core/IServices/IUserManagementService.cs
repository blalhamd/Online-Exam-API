using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.Dtos.Teacher;

namespace OnlineExam.Core.IServices
{
    public interface IUserManagementService
    {
        Task AddStudent(CreateStudentDto studentDto);
        Task AddTeacher(CreateTeacherDto teacherDto);
    }
}
