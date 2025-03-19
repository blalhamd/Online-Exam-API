namespace OnlineExam.Business.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepositoryAsync<Subject> _repositoryAsync;

        public SubjectService(IGenericRepositoryAsync<Subject> repositoryAsync)
        {
            _repositoryAsync = repositoryAsync;
        }

        public async Task<PaginatedResponse<Subject>> GetSubjectsAsync(int pageNumber = 1, int pageSize = 10)
        {
            (var query, var totalCount) = await _repositoryAsync.GetAllAsync(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var response = new PaginatedResponse<Subject>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            if (query is null)
            {
                response.Data = Enumerable.Empty<Subject>();
                return response;
            }

            response.Data = query;
            return response;
        }
    }
}
