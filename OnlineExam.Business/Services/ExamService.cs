namespace OnlineExam.Business.Services
{
    public class ExamService : IExamService
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ExamService> _logger;
        private readonly IValidator<CreateExamDto> _validator;
        private readonly IUnitOfWork<AppUser> _unitOfWork;
        private readonly IGenericRepositoryAsync<Exam> _examRepositoryAsync;
        private readonly IGenericRepositoryAsync<ChooseQuestion> _chooseQuestionRepositoryAsync;
        private readonly IGenericRepositoryAsync<Choice> _choiceRepositoryAsync;
        private readonly IGenericRepositoryAsync<TrueOrFalseQuestion> _trueOrFalseQuestionRepositoryAsync;
        private readonly IGenericRepositoryAsync<StudentExam> _studentExamGepositoryAsync;
        public ExamService(
            IMapper mapper,
            ILogger<ExamService> logger,
            IUnitOfWork<AppUser> unitOfWork,
            IValidator<CreateExamDto> validator,
            IGenericRepositoryAsync<Exam> examRepositoryAsync,
            IGenericRepositoryAsync<ChooseQuestion> chooseQuestionRepositoryAsync,
            IGenericRepositoryAsync<Choice> choiceRepositoryAsync,
            IGenericRepositoryAsync<TrueOrFalseQuestion> trueOrFalseQuestionRepositoryAsync,
            IGenericRepositoryAsync<StudentExam> studentExamGepositoryAsync)
        {
            _mapper = mapper;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _validator = validator;
            _examRepositoryAsync = examRepositoryAsync;
            _chooseQuestionRepositoryAsync = chooseQuestionRepositoryAsync;
            _choiceRepositoryAsync = choiceRepositoryAsync;
            _trueOrFalseQuestionRepositoryAsync = trueOrFalseQuestionRepositoryAsync;
            _studentExamGepositoryAsync = studentExamGepositoryAsync;
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
        public async Task<PaginatedResponse<ExamDto>> GetExams(int pageNumber = 1, int pageSize = 10)
        {
            (var query, var totalCount) = await _examRepositoryAsync.GetAllAsync(
                                         qu => qu.Include(x => x.ChooseQuestions)!
                                                 .ThenInclude(x => x.Choices)
                                                 .Include(x => x.TrueOrFalseQuestions)
                                                 .Include(x=> x.Subject), pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var response = new PaginatedResponse<ExamDto>
            {
                TotalPages = totalPages,
                PageSize = pageSize,
                CurrentPage = pageNumber
            };

            if (query is null)
            {
                response.Data = new List<ExamDto>();
                return response;
            }

            var examViewModels = _mapper.Map<IList<ExamDto>>(query);

            response.Data = examViewModels;
            return response;
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
                _logger.LogWarning("Validation failed for CreateExamViewModel: {Errors}", validationResult.Errors);
                throw new BadRequest(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // Fetch the exam including related entities
            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(
                e => e.Id == examId,
                query => query.Include(x => x.Subject)
            );

            if (exam is null)
            {
                _logger.LogWarning("EditExam failed - Exam with ID {ExamId} does not exist.", examId);
                throw new ItemNotFound($"Exam with ID {examId} does not exist.");
            }

            _mapper.Map(model, exam);

            await _examRepositoryAsync.UpdateEntityAsync(exam);
            
            var rowsAffected = await _unitOfWork.CommitAsync(cancellationToken);

            if (rowsAffected <= 0)
            {
                _logger.LogError("Failed to Edit the exam to the database.");
                throw new BadRequest("Exam couldn't be Updated.");
            }

            _logger.LogInformation("Exam Updated successfully");
        }

        public async Task<ExamDto> GetExamByIdAsync(int examId)
        {
            // Fetch the exam including related entities
            var exam = await _examRepositoryAsync.FirstOrDefaultAsync(
                e => e.Id == examId,
                query => query
                    .Include(x => x.TrueOrFalseQuestions)
                    .Include(x => x.ChooseQuestions)!
                    .ThenInclude(x=> x.Choices)
                    .Include(x => x.Subject)
            );

            // Throw exception if not found
            return exam is not null
                ? _mapper.Map<ExamDto>(exam)
                : throw new ItemNotFound($"Exam with ID {examId} does not exist");
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

        public async Task AddTrueOrFalseQuestionToExam(int examId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default)
        {
            var exam = await GetExamWithQuestions(examId, e => e.TrueOrFalseQuestions!);

            if (QuestionExists(exam.TrueOrFalseQuestions!,
                q => q.Title == model.Title && q.CorrectValue == model.CorrectValue))
                throw new ItemAlreadyExist("This question is already added");

            exam.TrueOrFalseQuestions ??= new List<TrueOrFalseQuestion>();
            exam.TrueOrFalseQuestions.Add(new TrueOrFalseQuestion
            {
                Title = model.Title,
                CorrectValue = model.CorrectValue,
                GradeOfQuestion = model.GradeOfQuestion
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

        public async Task UpdateTrueOrFalseQuestionToExam(int examId, int questionId,
            CreateTrueOrFalseQuestion model, CancellationToken cancellation = default)
        {
            var exam = await GetExamWithQuestions(examId, e => e.TrueOrFalseQuestions!);
            var question = exam.TrueOrFalseQuestions?.FirstOrDefault(x => x.Id == questionId)
                ?? throw new ItemNotFound("Question does not exist");

            question.Title = model.Title;
            question.GradeOfQuestion = model.GradeOfQuestion;
            question.CorrectValue = model.CorrectValue;

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

        public async Task DeleteTrueOrFalseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default)
        {
            var question = await _trueOrFalseQuestionRepositoryAsync.FirstOrDefaultAsync(x => x.Id == questionId && x.ExamId == examId);

            if (question is null) throw new ItemNotFound("Question does not exist");

            await _trueOrFalseQuestionRepositoryAsync.DeleteEntityAsync(question);

            var rowsAffected = await _unitOfWork.CommitAsync(cancellation);

            if (rowsAffected <= 0)
                throw new BadRequest($"Question didn't delete");
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
