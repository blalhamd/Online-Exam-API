using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Subject;

namespace OnlineExam.Core.IServices
{
    public interface ISubjectService
    {
        Task<PaginatedResponse<SubjectViewModel>> GetSubjectsAsync(int pageNumber = 1, int pageSize = 10);
    }  
}
