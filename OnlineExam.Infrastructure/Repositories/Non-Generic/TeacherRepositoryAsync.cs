using Microsoft.EntityFrameworkCore;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Domain.Entities;
using OnlineExam.Infrastructure.Data.context;
using OnlineExam.Infrastructure.Repositories.Generic;

namespace OnlineExam.Infrastructure.Repositories.Non_Generic
{
    public class TeacherRepositoryAsync : GenericRepositoryAsync<Teacher>, ITeacherRepositoryAsync
    {
        private readonly AppDbContext _context;
        public TeacherRepositoryAsync(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<TeacherDto>> GetTeachers(int pageNumber = 1, int pageSize = 10)
        {
            var teachers = _context.Teachers.AsQueryable();

            pageNumber = Math.Max(1, pageNumber); // Ensure positive page number
            var totalCount = await teachers.CountAsync();

            return new PaginatedResult<TeacherDto>
            {
                TotalCount = totalCount,
                Items = await teachers.Select(x => new TeacherDto
                {
                    Id = x.Id,
                    FullName = x.User.FullName,
                    Email = x.User.Email!,
                    PhoneNumber = x.User.PhoneNumber!,
                    HireDate = x.HireDate
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            };
        }
    }
}
