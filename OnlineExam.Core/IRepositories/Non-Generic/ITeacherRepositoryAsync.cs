using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Core.IRepositories.Non_Generic
{
    public interface ITeacherRepositoryAsync : IGenericRepositoryAsync<Teacher>
    {
        Task<PaginatedResult<TeacherDto>> GetTeachers(int pageNumber = 1, int pageSize = 10);
    }
}
