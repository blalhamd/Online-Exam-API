using Microsoft.EntityFrameworkCore;
using OnlineExam.Core.Dtos.Score;
using OnlineExam.Core.Dtos.UserAnswer;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Core.IServices;
using OnlineExam.Core.IUnit;
using OnlineExam.Domain.Entities;
using OnlineExam.Shared.Exceptions.Base;

namespace OnlineExam.Business.Services
{
    public class ExamAttemptService : IExamAttemptService
    {
        private readonly IGenericRepositoryAsync<ExamAttempt> _examAttemptRepositoryAsync;
        private readonly IGenericRepositoryAsync<Exam> _examRepositoryAsync;
        private readonly IGenericRepositoryAsync<ChooseQuestion> _chooseQuestionRepositoryAsync;
        private readonly IUnitOfWork<ExamAttempt> _unitOfWork;
        public ExamAttemptService(IGenericRepositoryAsync<ExamAttempt> examAttemptRepositoryAsync, IUnitOfWork<ExamAttempt> unitOfWork, IGenericRepositoryAsync<ChooseQuestion> chooseQuestionRepositoryAsync, IGenericRepositoryAsync<Exam> examRepositoryAsync)
        {
            _examAttemptRepositoryAsync = examAttemptRepositoryAsync;
            _unitOfWork = unitOfWork;
            _chooseQuestionRepositoryAsync = chooseQuestionRepositoryAsync;
            _examRepositoryAsync = examRepositoryAsync;
        }

        public async Task<int> StartExam(string userId, int examId)
        {
            var isExamExist = await _examRepositoryAsync.AnyAsync(x => x.Id == examId);

            if (!isExamExist)
                throw new ItemNotFound("Exam is not exist");

            var examAttemp = new ExamAttempt
            {
                ExamId = examId,
                UserId = userId,
                IsSubmitted = false,
                StartTime = DateTimeOffset.UtcNow,
            };

            await _examAttemptRepositoryAsync.AddEntityAsync(examAttemp);
            var rowsAffected = await _unitOfWork.CommitAsync();

            if (rowsAffected <= 0)
                throw new BadRequest("Exam Attempt is Failed");

            return examAttemp.Id;
        }

        public async Task<ScoreViewModel> SubmitExam(int examAttemptId, List<UserAnswerDto> answers, CancellationToken cancellation = default)
        {
            // 1. Load the exam attempt with existing answers
            var examAttempt = await _examAttemptRepositoryAsync
                .FirstOrDefaultAsync(
                    predicate: x => x.Id == examAttemptId,
                    include: q => q.Include(x => x.UserAnswers));

            if (examAttempt is null)
                throw new ItemNotFound("Exam attempt does not exist.");

            if (examAttempt.IsSubmitted)
                throw new BadRequest("Exam has already been submitted.");

            // 2. Get all valid questions for this exam
            var questionIds = answers.Select(a => a.QuestionId).Distinct().ToList();

            var examQuestions = await _chooseQuestionRepositoryAsync
                .GetAllAsync(q => questionIds.Contains(q.Id) && q.ExamId == examAttempt.ExamId);

            var questionDict = examQuestions.ToDictionary(q => q.Id);

            // 3. Initialize counters and result containers
            double totalScore = 0;
            double totalGrade = 0;
            int correctCount = 0;
            int wrongCount = 0;

            var userAnswers = new List<UserAnswer>();
            var answeredQuestions = new List<AnsweredQuestionViewModel>();

            // 4. Evaluate each answer
            foreach (var answer in answers)
            {
                if (!questionDict.TryGetValue(answer.QuestionId, out var question))
                    continue;

                bool isCorrect = answer.SelectedChoiceId == question.CorrectAnswerIndex;
                double score = isCorrect ? question.GradeOfQuestion : 0;

                userAnswers.Add(new UserAnswer
                {
                    ExamAttemptId = examAttemptId,
                    ChooseQuestionId = answer.QuestionId,
                    SelectedChoiceId = answer.SelectedChoiceId,
                    Score = score
                });

                answeredQuestions.Add(new AnsweredQuestionViewModel
                {
                    QuestionId = question.Id,
                    ExamId = question.ExamId,
                    Title = question.Title,
                    GradeOfQuestion = question.GradeOfQuestion,
                    IsCorrect = isCorrect
                });

                totalScore += score;
                totalGrade += question.GradeOfQuestion;
                if (isCorrect) correctCount++;
                else wrongCount++;
            }

            examAttempt.UserAnswers = userAnswers;
            examAttempt.Score = totalScore;
            examAttempt.IsSubmitted = true;
            examAttempt.EndTime = DateTimeOffset.UtcNow;

            await _examAttemptRepositoryAsync.UpdateEntityAsync(examAttempt);
            var rowsAffected = await _unitOfWork.CommitAsync(cancellation);

            if (rowsAffected <= 0)
                throw new BadRequest("Exam could not be submitted.");

            return new ScoreViewModel
            {
                Score = totalScore,
                TotalGrade = totalGrade,
                NumberCorrectQuestions = correctCount,
                NumberWrongQuestions = wrongCount,
                NumberQuestions = correctCount + wrongCount,
                ScoreInPercentage = totalGrade > 0 ? (totalScore / totalGrade) * 100 : 0,
                AnsweredQuestions = answeredQuestions
            };
        }

    }
}
