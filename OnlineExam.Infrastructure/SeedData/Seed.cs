using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineExam.Core.Constants;
using OnlineExam.Domain.Entities;
using OnlineExam.Domain.Entities.Identity;
using OnlineExam.Domain.Enums;
using OnlineExam.Infrastructure.Data.context;
using System.Security.Claims;

namespace OnlineExam.Infrastructure.SeedData
{
    public static class Seed
    {
        public static async Task Initialize(AppDbContext context, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {

            if (!await roleManager.Roles.AnyAsync())
            {


                foreach (var role in LoadRoles())
                {
                    if (!await roleManager.RoleExistsAsync(role.Name))
                    {
                        await roleManager.CreateAsync(role);
                    }
                }

                // Assign permissions to each role
                var rolePermissions = new Dictionary<string, List<string>>
        {
            {
                Role.Admin, new List<string>
                {
                    Permissions.Subjects.View,

                    Permissions.Exams.View, Permissions.Exams.ViewById, Permissions.Exams.Create, Permissions.Exams.Edit,
                    Permissions.Exams.Delete,

                    Permissions.Exams.AssignStudents,
                    Permissions.Exams.CreateChooseQuestion, Permissions.Exams.DeleteChooseQuestion, Permissions.Exams.EditChooseQuestion,
                    
                    Permissions.Users.AddStudent,
                    Permissions.Users.AddTeacher,
                }
            },
            {
                Role.Teacher, new List<string>
                {
                    Permissions.Subjects.View,

                    Permissions.Exams.View, Permissions.Exams.ViewById
                }
            },
            {
                Role.Student, new List<string>
                {
                    Permissions.Subjects.View,

                    Permissions.Exams.View, Permissions.Exams.ViewById
                }
            }
        };

                // Assign permissions to roles
                foreach (var rolePermission in rolePermissions)
                {
                    var roleName = rolePermission.Key;
                    var permissions = rolePermission.Value;

                    var role = await roleManager.FindByNameAsync(roleName);
                    if (role != null)
                    {
                        foreach (var permission in permissions)
                        {
                            if (!(await roleManager.RoleExistsAsync(role.Name)))
                            {
                                await roleManager.CreateAsync(new IdentityRole(role.Name));
                            }

                            // Check if the role already has the claim
                            var existingClaims = await roleManager.GetClaimsAsync(role);
                            if (!existingClaims.Any(c => c.Type == Permissions.Type && c.Value == permission))
                            {
                                await roleManager.AddClaimAsync(role, new Claim(Permissions.Type, permission));
                            }
                        }
                    }
                }

            }

            // Check if any users exist
            if (!await userManager.Users.AnyAsync())
            {
                var users = LoadUsers();

                foreach (var user in users)
                {
                    // Create user with password
                    var result = await userManager.CreateAsync(user, user.PasswordHash!);
                    if (result.Succeeded)
                    {
                        // Assign role to user
                        if (user.RoleType == Role.Admin)
                        {
                            await userManager.AddToRoleAsync(user, Role.Admin);
                        }
                        else if (user.RoleType == Role.Teacher)
                        {
                            var res = await userManager.AddToRoleAsync(user, Role.Teacher);

                            if (res.Succeeded)
                            {
                                var teacher = new Teacher
                                {
                                    UserId = user.Id,
                                    CreatedAt = DateTimeOffset.Now,
                                    HireDate = DateTimeOffset.Now,
                                };

                                await context.Teachers.AddAsync(teacher);
                                await context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            var res = await userManager.AddToRoleAsync(user, Role.Student);

                            if (res.Succeeded)
                            {
                                var student = new Student
                                {
                                    UserId = user.Id,
                                    CreatedAt = DateTimeOffset.Now,
                                };

                                await context.Students.AddAsync(student);
                                await context.SaveChangesAsync();
                            }
                        }
                    }
                }
            }

            if (!await context.Subjects.AnyAsync())
            {
                await context.Subjects.AddRangeAsync(LoadSubjects());
                await context.SaveChangesAsync();
            }

            if (!await context.Exams.AnyAsync())
            {
                var exams = LoadExams();

                foreach (var exam in exams)
                {
                    // ✅ Step 1: Temporarily detach dependent entities
                    var chooseQuestions = exam.ChooseQuestions;
                    exam.ChooseQuestions = null;

                    // ✅ Step 2: Save the Exam first
                    context.Exams.Add(exam);
                    context.SaveChanges(); // Now exam.Id is generated


                    // Step 4: Add Choose Questions with ExamId and null Choices
                    // Store Choices in a dictionary to access them later
                    var questionChoicesMap = new Dictionary<ChooseQuestion, List<Choice>>();
                    foreach (var chooseQuestion in chooseQuestions)
                    {
                        questionChoicesMap[chooseQuestion] = chooseQuestion.Choices.ToList(); // Store original Choices
                        chooseQuestion.ExamId = exam.Id;
                        chooseQuestion.Choices = null; // Prevent EF from saving Choices now
                        context.ChooseQuestions.Add(chooseQuestion);
                    }
                    context.SaveChanges(); // Save Choose Questions to generate Ids

                    // Step 5: Add Choices with QuestionId using stored Choices
                    foreach (var chooseQuestion in chooseQuestions)
                    {
                        var originalChoices = questionChoicesMap[chooseQuestion]; // Retrieve stored Choices
                        foreach (var choice in originalChoices)
                        {
                            choice.ChooseQuestionId = chooseQuestion.Id; // Link to generated Question Id
                            context.Choices.Add(choice);
                        }
                    }
                    context.SaveChanges(); // Save Choices
                }
            }

        }

        private static IEnumerable<IdentityRole> LoadRoles()
        {
            return new List<IdentityRole>()
            {
                new IdentityRole
                {
                    Name = Role.Admin,
                    NormalizedName = Role.NormlizedAdmin,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
                 new IdentityRole
                {
                    Name = Role.Student,
                    NormalizedName = Role.NormlizedStudent,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
                  new IdentityRole
                {
                    Name = Role.Teacher,
                    NormalizedName = Role.NormlizedTeacher,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
            };
        }

        private static IEnumerable<AppUser> LoadUsers()
        {
            return new List<AppUser>
    {
        // Admin user
        new AppUser
        {
            FullName = "Alexandra Carter",
            Email = "admin@example.com",
            NormalizedEmail = "ADMIN@EXAMPLE.COM",
            UserName = Role.Admin,
            NormalizedUserName = Role.NormlizedAdmin,
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0100",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Admin@2023", // Plaintext password (hash this in production)
            RoleType = Role.Admin,
        },
        // Student 1
        new AppUser
        {
            FullName = "Johnathan Doe",
            Email = "john.doe@example.com",
            NormalizedEmail = "JOHN.DOE@EXAMPLE.COM",
            UserName = "johndoe",
            NormalizedUserName = "JOHNDOE",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0101",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@123", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 2
        new AppUser
        {
            FullName = "Emily Rivera",
            Email = "emily.rivera@example.com",
            NormalizedEmail = "EMILY.RIVERA@EXAMPLE.COM",
            UserName = "emilyr",
            NormalizedUserName = "EMILYR",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0102",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@456", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 3
        new AppUser
        {
            FullName = "Michael Chen",
            Email = "michael.chen@example.com",
            NormalizedEmail = "MICHAEL.CHEN@EXAMPLE.COM",
            UserName = "michaelc",
            NormalizedUserName = "MICHAELC",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0103",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@789", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 4
        new AppUser
        {
            FullName = "Sophia Patel",
            Email = "sophia.patel@example.com",
            NormalizedEmail = "SOPHIA.PATEL@EXAMPLE.COM",
            UserName = "sophiap",
            NormalizedUserName = "SOPHIAP",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0104",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@101", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 5
        new AppUser
        {
            FullName = "Liam O’Connor",
            Email = "liam.oconnor@example.com",
            NormalizedEmail = "LIAM.OCONNOR@EXAMPLE.COM",
            UserName = "liamo",
            NormalizedUserName = "LIAMO",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0105",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@202", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 6
        new AppUser
        {
            FullName = "Ava Nguyen",
            Email = "ava.nguyen@example.com",
            NormalizedEmail = "AVA.NGUYEN@EXAMPLE.COM",
            UserName = "avan",
            NormalizedUserName = "AVAN",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0106",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@303", // Plaintext password
            RoleType = Role.Student,
        },
        // Student 7
        new AppUser
        {
            FullName = "Ethan Brooks",
            Email = "ethan.brooks@example.com",
            NormalizedEmail = "ETHAN.BROOKS@EXAMPLE.COM",
            UserName = "ethanb",
            NormalizedUserName = "ETHANB",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0107",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Student@404", // Plaintext password
            RoleType = Role.Student,
        },
        // Teacher 1
        new AppUser
        {
            FullName = "Dr. Sarah Mitchell",
            Email = "sarah.mitchell@example.com",
            NormalizedEmail = "SARAH.MITCHELL@EXAMPLE.COM",
            UserName = "sarahm",
            NormalizedUserName = "SARAHM",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0108",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@123", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 2
        new AppUser
        {
            FullName = "Prof. James Carter",
            Email = "james.carter@example.com",
            NormalizedEmail = "JAMES.CARTER@EXAMPLE.COM",
            UserName = "jamesc",
            NormalizedUserName = "JAMESC",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0109",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@456", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 3
        new AppUser
        {
            FullName = "Dr. Maria Lopez",
            Email = "maria.lopez@example.com",
            NormalizedEmail = "MARIA.LOPEZ@EXAMPLE.COM",
            UserName = "marial",
            NormalizedUserName = "MARIAL",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0110",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@789", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 4
        new AppUser
        {
            FullName = "Prof. David Kim",
            Email = "david.kim@example.com",
            NormalizedEmail = "DAVID.KIM@EXAMPLE.COM",
            UserName = "davidk",
            NormalizedUserName = "DAVIDK",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0111",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@101", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 5
        new AppUser
        {
            FullName = "Dr. Rachel Evans",
            Email = "rachel.evans@example.com",
            NormalizedEmail = "RACHEL.EVANS@EXAMPLE.COM",
            UserName = "rachele",
            NormalizedUserName = "RACHELE",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0112",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@202", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 6
        new AppUser
        {
            FullName = "Prof. Thomas Lee",
            Email = "thomas.lee@example.com",
            NormalizedEmail = "THOMAS.LEE@EXAMPLE.COM",
            UserName = "thomasl",
            NormalizedUserName = "THOMASL",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0113",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@303", // Plaintext password
            RoleType = Role.Teacher,
        },
        // Teacher 7
        new AppUser
        {
            FullName = "Dr. Olivia Grant",
            Email = "olivia.grant@example.com",
            NormalizedEmail = "OLIVIA.GRANT@EXAMPLE.COM",
            UserName = "oliviag",
            NormalizedUserName = "OLIVIAG",
            EmailConfirmed = true,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            PhoneNumber = "555-0114",
            SecurityStamp = Guid.NewGuid().ToString(),
            PasswordHash = "Teacher@404", // Plaintext password
            RoleType = Role.Teacher,
        }
    };
        }

        private static IEnumerable<Subject> LoadSubjects()
        {
            return new List<Subject>()
        {
            new Subject
            {
                Name = "Mathematics",
                Code = "MATH101",
                Description = "Basic calculus and algebra",
            },
            new Subject
            {
                Name = "Physics",
                Code = "PHYS201",
                Description = "Introduction to classical mechanics",
            },
            new Subject
            {
                Name = "Chemistry",
                Code = "CHEM101",
                Description = "Fundamentals of chemical reactions",
            },
            new Subject
            {
                Name = "Biology",
                Code = "BIO111",
                Description = "Basic cellular biology",
            },
            new Subject
            {
                Name = "Computer Science",
                Code = "CS101",
                Description = "Introduction to programming",
            },
            new Subject
            {
                Name = "English Literature",
                Code = "ENG201",
                Description = "Study of classic literature",
            },
            new Subject
            {
                Name = "History",
                Code = "HIST101",
                Description = "World history overview",
            },
            new Subject
            {
                Name = "Psychology",
                Code = "PSY101",
                Description = "Introduction to human behavior",
            },
            new Subject
            {
                Name = "Economics",
                Code = "ECON201",
                Description = "Principles of microeconomics",
            },
            new Subject
            {
                Name = "Art History",
                Code = "ART101",
                Description = "Survey of art through the ages",
            }
            };
        }

        private static IEnumerable<Exam> LoadExams()
        {
            var exam = new Exam
            {
                ExamType = ExamType.MidTerm,
                Description = "Mathematics Midterm Examination",
                Duration = TimeOnly.FromTimeSpan(TimeSpan.FromHours(2)),
                Level = 2,
                Status = true,
                TotalGrade = 50,
                SubjectId = 1,
                ChooseQuestions = new List<ChooseQuestion>()
            };


            // Add Choose Questions with Choices
            var chooseQuestions = new[]
            {
                new ChooseQuestion
                {
                    Title = "What is 5 + 3?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 2,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "6" },
                        new Choice { Text = "7" },
                        new Choice { Text = "8" },
                        new Choice { Text = "9" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is the square of 4?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 1,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "12" },
                        new Choice { Text = "16" },
                        new Choice { Text = "18" },
                        new Choice { Text = "20" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is 10 divided by 2?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 0,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "5" },
                        new Choice { Text = "4" },
                        new Choice { Text = "2" },
                        new Choice { Text = "10" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is the value of π approximately?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 3,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "2.14" },
                        new Choice { Text = "3" },
                        new Choice { Text = "3.41" },
                        new Choice { Text = "3.14" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "How many sides does a pentagon have?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 1,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "4" },
                        new Choice { Text = "5" },
                        new Choice { Text = "6" },
                        new Choice { Text = "7" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is 3 × 4?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 2,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "10" },
                        new Choice { Text = "11" },
                        new Choice { Text = "12" },
                        new Choice { Text = "13" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is the cube of 2?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 0,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "8" },
                        new Choice { Text = "6" },
                        new Choice { Text = "4" },
                        new Choice { Text = "10" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is 15 - 7?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 1,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "6" },
                        new Choice { Text = "8" },
                        new Choice { Text = "9" },
                        new Choice { Text = "7" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is half of 16?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 3,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "6" },
                        new Choice { Text = "7" },
                        new Choice { Text = "9" },
                        new Choice { Text = "8" }
                    }
                },
                new ChooseQuestion
                {
                    Title = "What is 2 + 3 × 4?",
                    GradeOfQuestion = 2,
                    CorrectAnswerIndex = 2,
                    Choices = new List<Choice>
                    {
                        new Choice { Text = "20" },
                        new Choice { Text = "10" },
                        new Choice { Text = "14" },
                        new Choice { Text = "12" }
                    }
                }
            };

            exam.ChooseQuestions.AddRange(chooseQuestions);

            return new List<Exam> { exam };
        }
    }
}
