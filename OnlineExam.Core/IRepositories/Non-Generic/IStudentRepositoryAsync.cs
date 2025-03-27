using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Core.IRepositories.Non_Generic
{
    public interface IStudentRepositoryAsync : IGenericRepositoryAsync<Student>
    {
        Task<PaginatedResult<StudentDto>> GetStudents(int pageNumber = 1, int pageSize = 10);
    }
}
