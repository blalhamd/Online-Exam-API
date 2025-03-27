using Microsoft.EntityFrameworkCore;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Domain.Entities;
using OnlineExam.Infrastructure.Data.context;
using OnlineExam.Infrastructure.Repositories.Generic;

namespace OnlineExam.Infrastructure.Repositories.Non_Generic
{
    public class StudentRepositoryAsync : GenericRepositoryAsync<Student>, IStudentRepositoryAsync
    {
        private readonly AppDbContext _context;
        public StudentRepositoryAsync(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<StudentDto>> GetStudents(int pageNumber = 1, int pageSize = 10)
        {
            var students = _context.Students.AsQueryable();

            pageNumber = Math.Max(1, pageNumber); // Ensure positive page number
            var totalCount = await students.CountAsync();

            return new PaginatedResult<StudentDto>
            {
                TotalCount = totalCount,
                Items = await students.Select(x => new StudentDto
                {
                    Id = x.Id,
                    FullName = x.User.FullName,
                    Email = x.User.Email!,
                    PhoneNumber = x.User.PhoneNumber!
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(),
            };
        }
    }
}
