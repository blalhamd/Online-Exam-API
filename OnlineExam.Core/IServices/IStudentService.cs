using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;

namespace OnlineExam.Core.IServices
{
    public interface IStudentService
    {
        Task AddStudent(CreateStudentDto studentDto);
        Task<PaginatedResponse<StudentViewModel>> GetStudents(int pageNumber = 1, int pageSize = 10);
    }
}
