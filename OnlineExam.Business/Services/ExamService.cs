using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExam.Core.Dtos.Choose.Requests;
using OnlineExam.Core.Dtos.Exam.Request;
using OnlineExam.Core.Dtos.Exam.Response;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Core.IServices;
using OnlineExam.Core.IUnit;
using OnlineExam.Domain.Entities;
using OnlineExam.Domain.Entities.Identity;
using OnlineExam.Shared.Exceptions.Base;
using System.Linq.Expressions;

namespace OnlineExam.Business.Services
{
    public class ExamService : IExamService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ExamService> _logger;
        private readonly IValidator<CreateExamDto> _validator;
        private readonly IUnitOfWork<AppUser> _unitOfWork;
        private readonly IExamRepositoryAsync _examRepositoryAsync;
        private readonly IGenericRepositoryAsync<ChooseQuestion> _chooseQuestionRepositoryAsync;
        private readonly IGenericRepositoryAsync<Choice> _choiceRepositoryAsync;
        private readonly IGenericRepositoryAsync<StudentExam> _studentExamGepositoryAsync;
        private readonly IGenericRepositoryAsync<UserAnswer> _userAnswerExamGepositoryAsync;
        public ExamService(
            IMapper mapper,
            ILogger<ExamService> logger,
            IUnitOfWork<AppUser> unitOfWork,
            IValidator<CreateExamDto> validator,
            IExamRepositoryAsync examRepositoryAsync,
            IGenericRepositoryAsync<ChooseQuestion> chooseQuestionRepositoryAsync,
            IGenericRepositoryAsync<Choice> choiceRepositoryAsync,
            IGenericRepositoryAsync<StudentExam> studentExamGepositoryAsync,
            IGenericRepositoryAsync<UserAnswer> userAnswerExamGepositoryAsync)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _examRepositoryAsync = examRepositoryAsync;
            _chooseQuestionRepositoryAsync = chooseQuestionRepositoryAsync;
            _choiceRepositoryAsync = choiceRepositoryAsync;
            _studentExamGepositoryAsync = studentExamGepositoryAsync;
            _userAnswerExamGepositoryAsync = userAnswerExamGepositoryAsync;
        }

        public async Task AssignStudentsToExam(int examId, List<int> studentIds, CancellationToken cancellation = default)
        {
            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(x => x.Id == examId);

            if (exam is null)
            {
                _logger.LogWarning("Exam does not exist");
                throw new ItemNotFound("Exam does not exist");
            }

            List<StudentExam> studentExams = new List<StudentExam>();

            foreach (var studentId in studentIds)
            {
                studentExams.Add(new StudentExam
                {
                    ExamId = examId,
                    StudentId = studentId,
                    IsCompleted = false,
                });
            }

            await _studentExamGepositoryAsync.AddRangeAsync(studentExams);
            var rowsAffected = await _unitOfWork.CommitAsync(cancellation);

            if (rowsAffected <= 0)
            {
                _logger.LogWarning("there are wrong happen");
                throw new BadRequest("there are wrong happen");
            }
        }
        public async Task<PaginatedResponse<ExamViewModel>> GetExams(int pageNumber = 1, int pageSize = 10)
        {
            var paginatedResult = await _examRepositoryAsync.GetExams(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)paginatedResult.TotalCount / pageSize);

            var response = new PaginatedResponse<ExamViewModel>
            {
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };

            if (paginatedResult.Items is null)
            {
                response.Data = new List<ExamViewModel>();
                return response;
            }

            var examViewModels = _mapper.Map<IList<ExamViewModel>>(paginatedResult.Items);

            response.Data = examViewModels;
            return response;
        }

        public async Task<ExamViewModel> GetExamByIdAsync(int examId)
        {
            var exam = await _examRepositoryAsync.GetExamById(examId);

            // Throw exception if not found
            return exam is not null
                ? _mapper.Map<ExamViewModel>(exam)
                : throw new ItemNotFound($"Exam with ID {examId} does not exist");
        }


        public async Task CreateExam(CreateExamDto model, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateExamViewModel: {Errors}", validationResult.Errors);
                throw new BadRequest(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var exam = _mapper.Map<Exam>(model);

            // Save to database
            await _examRepositoryAsync.AddEntityAsync(exam);
            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);

            if (rowsAffected <= 0)
            {
                _logger.LogError("Failed to add the exam to the database.");
                throw new BadRequest("Exam couldn't be added.");
            }

            _logger.LogInformation("Exam created successfully");
        }

        

        public async Task EditExam(int examId, CreateExamDto model, CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(model, cancellationToken);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateExamDto: {Errors}", validationResult.Errors);
                throw new BadRequest(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(
                e => e.Id == examId,
                query => query.Include(x => x.ChooseQuestions)
                              .ThenInclude(q => q.Choices)
            );

            if (exam is null)
            {
                _logger.LogWarning("EditExam failed - Exam with ID {ExamId} does not exist.", examId);
                throw new ItemNotFound($"Exam with ID {examId} does not exist.");
            }

            // ⚠️ Remove related UserAnswers manually before modifying choices
            var relatedQuestionIds = exam.ChooseQuestions.Select(q => q.Id).ToList();

            var userAnswers = await _userAnswerExamGepositoryAsync
                .GetAllAsync(a => relatedQuestionIds.Contains(a.ChooseQuestionId));

            await _userAnswerExamGepositoryAsync.DeleteRangeAsync(userAnswers);

            // Remove old questions (with their choices via Cascade)
            await _chooseQuestionRepositoryAsync.DeleteRangeAsync(exam.ChooseQuestions);

            // Update exam basic data
            exam.SubjectId = model.SubjectId;
            exam.TotalGrade = model.TotalGrade;
            exam.Level = model.Level;
            exam.Description = model.Description;
            exam.Status = model.Status;
            exam.Duration = model.Duration;
            exam.ExamType = model.ExamType;

            // Add new choose questions with choices
            foreach (var qDto in model.ChooseQuestions)
            {
                var question = new ChooseQuestion
                {
                    Title = qDto.Title,
                    GradeOfQuestion = qDto.GradeOfQuestion,
                    CorrectAnswerIndex = qDto.CorrectAnswerIndex,
                    ExamId = exam.Id,
                    Choices = qDto.Choices.Select(c => new Choice { Text = c.Text }).ToList()
                };

                exam.ChooseQuestions.Add(question);
            }

            await _examRepositoryAsync.UpdateEntityAsync(exam);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);
            if (rowsAffected <= 0)
            {
                _logger.LogError("Failed to update the exam.");
                throw new BadRequest("Exam couldn't be updated.");
            }

            _logger.LogInformation("Exam ID {ExamId} updated successfully.", examId);
        }


        public async Task DeleteExam(int examId, CancellationToken cancellationToken = default)
        {
            // Fetch the exam including related entities
            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(e => e.Id == examId);

            if (exam is null)
                throw new ItemNotFound($"Exam does not exist");

            await _examRepositoryAsync.DeleteEntityAsync(exam);
            await _unitOfWork.CommitAsync(cancellationToken);
        }

       
        public async Task AddChooseQuestionToExam(int examId, CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            var exam = await GetExamWithQuestions(examId, e => e.ChooseQuestions!);

            if (QuestionExists(exam.ChooseQuestions!,
                q => q.Title == model.Title && q.Choices.SequenceEqual(model.Choices.Select(c => new Choice { Text = c.Text }))))
                throw new ItemAlreadyExist("This question is already added");

            exam.ChooseQuestions ??= new List<ChooseQuestion>();
            exam.ChooseQuestions.Add(new ChooseQuestion
            {
                Title = model.Title,
                Choices = model.Choices.Select(x => new Choice { Text = x.Text }).ToList(),
                GradeOfQuestion = model.GradeOfQuestion,
                CorrectAnswerIndex = model.CorrectAnswerIndex
            });

            await CommitChangesAsync(exam, cancellation, "add");
        }

      

        // Update Methods
        public async Task UpdateChooseQuestionToExam(int examId, int questionId,
            CreateChooseQuestionDto model, CancellationToken cancellation = default)
        {
            var exam = await GetExamWithQuestions(examId, e => e.ChooseQuestions!);
            var question = exam.ChooseQuestions?.FirstOrDefault(x => x.Id == questionId)
                ?? throw new ItemNotFound("Question does not exist");

            question.Title = model.Title;
            question.GradeOfQuestion = model.GradeOfQuestion;
            question.Choices = model.Choices.Select(x => new Choice { Text = x.Text }).ToList();
            question.CorrectAnswerIndex = model.CorrectAnswerIndex;

            await CommitChangesAsync(exam, cancellation, "update");
        }

      

        // Delete Methods (Fixed to remove specific questions)
        public async Task DeleteChooseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default)
        {
            var question = await _chooseQuestionRepositoryAsync.FirstOrDefaultAsync(q => q.Id == questionId && q.ExamId == examId);
            if (question is null)
                throw new ItemNotFound("Question is not found");

            var choices = await _choiceRepositoryAsync.GetAllAsync(x => x.ChooseQuestionId == question.Id);
            foreach (var choice in choices)
            {
                await _choiceRepositoryAsync.DeleteEntityAsync(choice);
            }
            await _unitOfWork.CommitAsync(cancellation);

            await _chooseQuestionRepositoryAsync.DeleteEntityAsync(question);
            var rowsAffected = await _unitOfWork.CommitAsync(cancellation);

            if (rowsAffected <= 0)
                throw new BadRequest("question is not exist");
        }

        

        // Generic method to get exam with included questions
        private async Task<Exam> GetExamWithQuestions<T>(int examId, Expression<Func<Exam, IEnumerable<T>>> include)
            where T : class
        {
            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(
                e => e.Id == examId,
                query => query.Include(include));

            if (exam == null)
                throw new ItemNotFound("Exam does not exist");

            return exam;
        }

        // Generic method to check if question exists
        private static bool QuestionExists<T>(IEnumerable<T> questions, Func<T, bool> existsPredicate)
            where T : class
        {
            return questions?.Any(existsPredicate) ?? false;
        }

        // Generic method to commit changes
        private async Task CommitChangesAsync(Exam exam, CancellationToken cancellation, string operation)
        {
            await _examRepositoryAsync.UpdateEntityAsync(exam);
            var rowsAffected = await _unitOfWork.CommitAsync(cancellation);

            if (rowsAffected <= 0)
                throw new BadRequest($"Question didn't {operation}");
        }

        
    }
}
