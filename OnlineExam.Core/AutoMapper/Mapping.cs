using AutoMapper;
using OnlineExam.Core.Dtos.Choose.Requests;
using OnlineExam.Core.Dtos.Choose.Responses;
using OnlineExam.Core.Dtos.Exam.Request;
using OnlineExam.Core.Dtos.Exam.Response;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.Dtos.Subject;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Domain.Entities;

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

            CreateMap<ExamDto,ExamViewModel>().ReverseMap();

            CreateMap<ChooseQuestion, ChooseQuestionDto>().ReverseMap();

            CreateMap<ExamDto, CreateExamDto>().ReverseMap();

            CreateMap<Choice, ChoiceDto>().ReverseMap();
            CreateMap<CreateChoiceDto,Choice>();

            CreateMap<ChooseQuestionDto, CreateChooseQuestionDto>().ReverseMap();
            CreateMap<ChoiceDto, CreateChoiceDto>().ReverseMap();

            CreateMap<Subject, SubjectViewModel>().ReverseMap();
            CreateMap<Teacher, TeacherViewModel>().ReverseMap();
            CreateMap<Student, StudentViewModel>().ReverseMap();

        }
    }
}
