using System.Runtime.Serialization;

namespace OnlineExam.Domain.Enums
{
    public enum ExamType
    {
        [EnumMember(Value = "Quiz")]
        Quiz = 0,

        [EnumMember(Value = "Final")]
        Final = 1,

        [EnumMember(Value = "MidTerm")]
        MidTerm = 2,

        [EnumMember(Value = "Practice")]
        Practice = 3
    }
}
