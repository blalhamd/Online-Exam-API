using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineExam.Core.Constants;
using OnlineExam.Core.Dtos.Pagination;
using OnlineExam.Core.Dtos.Student;
using OnlineExam.Core.Dtos.Teacher;
using OnlineExam.Core.IRepositories.Generic;
using OnlineExam.Core.IRepositories.Non_Generic;
using OnlineExam.Core.IServices;
using OnlineExam.Core.IServices.Email;
using OnlineExam.Core.IUnit;
using OnlineExam.Domain.Entities;
using OnlineExam.Domain.Entities.Identity;
using OnlineExam.Shared.Exceptions.Base;

namespace OnlineExam.Business.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUnitOfWork<Student> _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IStudentRepositoryAsync _studentRepository;
        private readonly ITeacherRepositoryAsync _teacherRepository;
        private readonly IValidator<CreateStudentDto> _validator;
        private readonly IValidator<CreateTeacherDto> _teacherValidator;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;

        public UserManagementService(
            IUnitOfWork<Student> unitOfWork,
            UserManager<AppUser> userManager,
            ITeacherRepositoryAsync teacherRepository,
            IStudentRepositoryAsync studentRepository,
            IValidator<CreateStudentDto> validator,
            IValidator<CreateTeacherDto> teacherValidator,
            IEmailBodyBuilder emailBodyBuilder,
            IEmailSender emailSender,
            ILogger<UserManagementService> logger,
            IMapper mapper,
            IHttpContextAccessor contextAccessor)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _teacherRepository = teacherRepository;
            _studentRepository = studentRepository;
            _validator = validator;
            _teacherValidator = teacherValidator;
            _emailBodyBuilder = emailBodyBuilder;
            _emailSender = emailSender;
            _logger = logger;
            _mapper = mapper;
            _contextAccessor = contextAccessor;
        }

        public async Task AddStudent(CreateStudentDto studentDto)
        {
            var validationResult = await _validator.ValidateAsync(studentDto);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateStudentDto: {Errors}", validationResult.Errors);
                throw new BadRequest(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var isExist = await _userManager.Users.AnyAsync(u => u.Email == studentDto.Email && u.FullName == studentDto.FullName);

            if (isExist)
            {
                _logger.LogWarning("Attempt to create a duplicate student: {Email}, {Name}", studentDto.Email, studentDto.FullName);
                throw new ItemAlreadyExist("Attempt to create a duplicate student");
            }

            var user = new AppUser
            {
                FullName = studentDto.FullName,
                Email = studentDto.Email,
                RoleType = Role.Student,
                UserName = studentDto.Email.Split('@').FirstOrDefault() ?? studentDto.Email,
                PhoneNumber = studentDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, studentDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create student user: {Errors}", errors);
                throw new BadRequest(errors);
            }

            await _userManager.AddToRoleAsync(user, Role.Student);

            var student = new Student
            {
                UserId = user.Id
            };

            await _studentRepository.AddEntityAsync(student);
            await _unitOfWork.CommitAsync();

            // Sending Email to Student with Email and Password to login in the system.

            await SendEmail(user,studentDto.Password);

            _logger.LogInformation("Student created successfully: {UserId} ({Email})", user.Id, user.Email);
        }

        public async Task AddTeacher(CreateTeacherDto teacherDto)
        {
            var validationResult = await _teacherValidator.ValidateAsync(teacherDto);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation failed for CreateTeacherDto: {Errors}", validationResult.Errors);
                throw new BadRequest(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            var isExist = await _userManager.Users.AnyAsync(u => u.Email == teacherDto.Email && u.FullName == teacherDto.FullName);

            if (isExist)
            {
                _logger.LogWarning("Attempt to create a duplicate Teacher: {Email}, {Name}", teacherDto.Email, teacherDto.FullName);
                throw new ItemAlreadyExist("Attempt to create a duplicate Teacher");
            }

            var user = new AppUser
            {
                FullName = teacherDto.FullName,
                Email = teacherDto.Email,
                RoleType = Role.Teacher,
                UserName = teacherDto.Email.Split('@').FirstOrDefault() ?? teacherDto.Email,
                PhoneNumber = teacherDto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, teacherDto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create teacher user: {Errors}", errors);
                throw new BadRequest(errors);
            }

            await _userManager.AddToRoleAsync(user, Role.Teacher);

            var teacher = new Teacher
            {
                UserId = user.Id,
                HireDate = teacherDto.HireDate,
                TeacherSubjects = teacherDto.Subjects.Select(s => new TeacherSubject
                {
                    Subject = new Subject
                    {
                        Name = s.Name,
                        Description = s.Description,
                        Code = s.Code
                    }
                }).ToList()
            };

            await _teacherRepository.AddEntityAsync(teacher);
            await _unitOfWork.CommitAsync();

            // Sending Email to teacher with Email and Password to login in the system.

            await SendEmail(user,teacherDto.Password);

            _logger.LogInformation("Teacher created successfully: {UserId} ({Email})", user.Id, user.Email);
        }

        public async Task<PaginatedResponse<StudentViewModel>> GetStudents(int pageNumber = 1, int pageSize = 10)
        {
            var pagienedResult = await _studentRepository.GetStudents(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)pagienedResult.TotalCount / pageSize);

            var response = new PaginatedResponse<StudentViewModel>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            if (pagienedResult is null)
            {
                response.Data = new List<StudentViewModel>();
                return response;
            }

            var studentsViewModel = _mapper.Map<List<StudentViewModel>>(pagienedResult.Items);

            response.Data = studentsViewModel;

            return response;
        }

        public async Task<PaginatedResponse<TeacherViewModel>> GetTeachers(int pageNumber = 1, int pageSize = 10)
        {
            var pagienedResult = await _teacherRepository.GetTeachers(pageNumber, pageSize);

            var totalPages = (int)Math.Ceiling((double)pagienedResult.TotalCount / pageSize);

            var response = new PaginatedResponse<TeacherViewModel>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
            };

            if (pagienedResult is null)
            {
                response.Data = new List<TeacherViewModel>();
                return response;
            }

            var teachersViewModel = _mapper.Map<List<TeacherViewModel>>(pagienedResult.Items);

            response.Data = teachersViewModel;

            return response;
        }

        private async Task SendEmail(AppUser user,string password)
        {
            var origin = _contextAccessor?.HttpContext?.Request.Headers.Origin;
            var imagePath = $"{origin}/images/Anatomy-of-the-Perfect-Thank-You-Page.png";

            var body = await _emailBodyBuilder.GenerateEmailBody(
                templateName: "emailTamplate.html",
                imageUrl: imagePath,
                header: $"Hi, {user.FullName.Split(' ')[0]}",
                TextBody: $"Email: {user.Email} \nPassword: {password}",
                link: $"{origin}/auth/login",
                linkTitle: "Login");

            await _emailSender.SendEmailAsync(user.Email!, "✅ Online Exam: Your Account", body);
        }
    }
}
