using OnlineExam.Core.Dtos.Exam.Response;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Core.IRepositories.Non_Generic
{
    public interface IExamRepositoryAsync : IGenericRepositoryAsync<Exam>
    {
        Task<PaginatedResult<ExamDto>> GetExams(int pageNumber = 1, int pageSize = 1);
        Task<ExamDto> GetExamById(int examId);
    }
}
