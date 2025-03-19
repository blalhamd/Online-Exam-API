namespace OnlineExam.Core.IServices
{
    public interface IExamService
    {
        Task AssignStudentsToExam(int examId, List<int> studentIds, CancellationToken cancellation = default);
        Task CreateExam(CreateExamDto model, CancellationToken cancellationToken = default);
        Task EditExam(int examId, CreateExamDto model, CancellationToken cancellationToken = default);
        Task DeleteExam(int examId, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<ExamDto>> GetExams(int pageNumber = 1, int pageSize = 1);
        Task<ExamDto> GetExamByIdAsync(int examId);
        Task AddChooseQuestionToExam(int examId, CreateChooseQuestionDto model, CancellationToken cancellation = default);
        Task AddTrueOrFalseQuestionToExam(int examId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default);
        Task UpdateChooseQuestionToExam(int examId, int questionId, CreateChooseQuestionDto model, CancellationToken cancellation = default);
        Task UpdateTrueOrFalseQuestionToExam(int examId, int questionId, CreateTrueOrFalseQuestion model, CancellationToken cancellation = default);
        Task DeleteChooseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default);
        Task DeleteTrueOrFalseQuestionToExam(int examId, int questionId, CancellationToken cancellation = default);
    }
}
