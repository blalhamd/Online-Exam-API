using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Core.Dtos.Choose.Responses;
using OnlineExam.Core.Dtos.Exam.Response;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Domain.Entities;
using OnlineExam.Infrastructure.Data.context;
using OnlineExam.Infrastructure.Repositories.Generic;

namespace OnlineExam.Infrastructure.Repositories.Non_Generic
{
    public class ExamRepositoryAsync : GenericRepositoryAsync<Exam>, IExamRepositoryAsync
    {
        private readonly AppDbContext _context;
        public ExamRepositoryAsync(AppDbContext context, IMapper mapper) : base(context)
        {
            _context = context;
        }


        public async Task<PaginatedResult<ExamDto>> GetExams(int pageNumber = 1, int pageSize = 1)
        {
            pageNumber = Math.Max(1, pageNumber);

            var examsDto = _context.Exams.Select(x => new ExamDto
            {
                Id = x.Id,
                Description = x.Description,
                Duration = x.Duration,
                ExamType = x.ExamType,
                Level = x.Level,
                Status = x.Status,
                TotalGrade = x.TotalGrade,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject.Name,
                ChooseQuestions = x.ChooseQuestions.Select(q => new ChooseQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    ExamId = q.ExamId,
                    GradeOfQuestion = q.GradeOfQuestion,
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Choices = q.Choices.Select(c => new ChoiceDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                    }).ToList(),
                }).ToList()
            }).AsQueryable();
             
            var totalCount = examsDto.Count();

            var exams = await examsDto.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedResult<ExamDto>
            {
                Items = exams,
                TotalCount = totalCount,
            };
        }


        public async Task<ExamDto> GetExamById(int examId)
        {
            var examDto = await _context.Exams.Where(x => x.Id == examId).Select(x => new ExamDto
            {
                Id = x.Id,
                Description = x.Description,
                Duration = x.Duration,
                ExamType = x.ExamType,
                Level = x.Level,
                Status = x.Status,
                TotalGrade = x.TotalGrade,
                SubjectId = x.SubjectId,
                SubjectName = x.Subject.Name,
                ChooseQuestions = x.ChooseQuestions.Select(q => new ChooseQuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    ExamId = q.ExamId,
                    GradeOfQuestion = q.GradeOfQuestion,
                    CorrectAnswerIndex = q.CorrectAnswerIndex,
                    Choices = q.Choices.Select(c => new ChoiceDto
                    {
                        Id = c.Id,
                        Text = c.Text,
                    }).ToList(),
                }).ToList()
            }).FirstOrDefaultAsync();

            return examDto;
        }
    }
}
