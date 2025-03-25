using AutoMapper;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Subject;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Core.IServices;
using OnlineExam.Domain.Entities;

namespace OnlineExam.Business.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly IGenericRepositoryAsync<Subject> _repositoryAsync;
        private readonly IMapper _mapper;
        public SubjectService(IGenericRepositoryAsync<Subject> repositoryAsync, IMapper mapper)
        {
            _repositoryAsync = repositoryAsync;
            _mapper = mapper;
        }

        public async Task<PaginatedResponse<SubjectViewModel>> GetSubjectsAsync(int pageNumber = 1, int pageSize = 10)
        {
            var paginatedResult = await _repositoryAsync.GetAllAsync(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)paginatedResult.TotalCount / pageSize);

            var response = new PaginatedResponse<SubjectViewModel>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            if (paginatedResult.Items is null)
            {
                response.Data = Enumerable.Empty<SubjectViewModel>();
                return response;
            }

            response.Data = _mapper.Map<List<SubjectViewModel>>(paginatedResult.Items);

            return response;
        }
    }
}
