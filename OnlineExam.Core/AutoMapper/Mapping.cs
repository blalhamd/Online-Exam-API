namespace OnlineExam.Core.AutoMapper
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            // ✅ Mapping for CreateExam action
            CreateMap<CreateExamDto, Exam>();

            CreateMap<CreateChooseQuestionDto, ChooseQuestion>();
            CreateMap<CreateChoiceDto, Choice>().ReverseMap(); // ✅ Bidirectional mapping
            
            // ✅ Mapping for retrieving an exam (Exam → ExamDto)
            CreateMap<Exam, ExamDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(x => x.Subject.Name))
                .ForMember(dest => dest.ChooseQuestions, opt => opt.MapFrom(x => x.ChooseQuestions));

            CreateMap<ChooseQuestion, ChooseQuestionDto>().ReverseMap();
            CreateMap<TrueOrFalseQuestion, CreateTrueOrFalseQuestion>().ReverseMap();

            CreateMap<ExamDto, CreateExamDto>().ReverseMap();

            CreateMap<Choice, ChoiceDto>().ReverseMap();
            CreateMap<CreateChoiceDto,Choice>();

            CreateMap<ChooseQuestionDto, CreateChooseQuestionDto>().ReverseMap();
            CreateMap<ChoiceDto, CreateChoiceDto>().ReverseMap();

            CreateMap<TrueOrFalseQuestionDto, TrueOrFalseQuestion>().ReverseMap();
        }
    }
}
