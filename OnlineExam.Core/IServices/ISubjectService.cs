namespace OnlineExam.Core.IServices
{
    public interface ISubjectService
    {
        Task<PaginatedResponse<Subject>> GetSubjectsAsync(int pageNumber = 1, int pageSize = 10);
    }  
}
