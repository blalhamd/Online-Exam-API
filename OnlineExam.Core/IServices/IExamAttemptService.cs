using OnlineExam.Core.Dtos.Score;
using OnlineExam.Core.Dtos.UserAnswer;

namespace OnlineExam.Core.IServices
{
    public interface IExamAttemptService
    {
        Task<int> StartExam(string userId, int examId);
        Task<ScoreViewModel> SubmitExam(int examAttempId, List<UserAnswerDto> answers, CancellationToken cancellation = default);
    }
}
