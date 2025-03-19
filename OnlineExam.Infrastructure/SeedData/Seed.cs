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
                    await roleManager.CreateAsync(role);
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
                        if (user.Email == "admin@example.com")
                        {
                            await userManager.AddToRoleAsync(user, "Admin");
                        }
                        else
                        {
                            var res = await userManager.AddToRoleAsync(user, "User");

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
                    var tfQuestions = exam.TrueOrFalseQuestions;
                    var chooseQuestions = exam.ChooseQuestions;
                    exam.TrueOrFalseQuestions = null;
                    exam.ChooseQuestions = null;

                    // ✅ Step 2: Save the Exam first
                    context.Exams.Add(exam);
                    context.SaveChanges(); // Now exam.Id is generated

                    // ✅ Step 3: Reattach True/False Questions with ExamId & Save
                    foreach (var tfQuestion in tfQuestions!)
                    {
                        tfQuestion.ExamId = exam.Id;
                        context.TrueOrFalseQuestions.Add(tfQuestion);
                    }
                    context.SaveChanges(); // Save True/False Questions

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
            return new List<IdentityRole>
        {
            new IdentityRole
            {
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            },
            new IdentityRole
            {
                Name = "User",
                NormalizedName = "USER",
                ConcurrencyStamp = Guid.NewGuid().ToString()
            }
        };
        }

        private static IEnumerable<AppUser> LoadUsers()
        {
            return new List<AppUser>
        {
            // Admin user
            new AppUser
            {
                FullName = "Admin User",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = "Admin@123" // This will be the password (not hashed yet)
            },
            // Regular user 1
            new AppUser
            {
                FullName = "John Doe",
                Email = "john@example.com",
                NormalizedEmail = "JOHN@EXAMPLE.COM",
                UserName = "john",
                NormalizedUserName = "JOHN",
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567891",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = "User@123" // This will be the password (not hashed yet)
            },
            // Regular user 2
            new AppUser
            {
                FullName = "Jane Smith",
                Email = "jane@example.com",
                NormalizedEmail = "JANE@EXAMPLE.COM",
                UserName = "jane",
                NormalizedUserName = "JANE",
                EmailConfirmed = true,
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567892",
                SecurityStamp = Guid.NewGuid().ToString(),
                PasswordHash = "User@123" // This will be the password (not hashed yet)
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
                TrueOrFalseQuestions = new List<TrueOrFalseQuestion>(),
                ChooseQuestions = new List<ChooseQuestion>()
            };

            // Add True/False Questions
            var tfQuestions = new[]
            {
               new TrueOrFalseQuestion { Title = "2 + 2 equals 4", GradeOfQuestion = 1, CorrectValue = true },
               new TrueOrFalseQuestion { Title = "The square root of 9 is 4", GradeOfQuestion = 1, CorrectValue = false },
               new TrueOrFalseQuestion { Title = "A circle has 360 degrees", GradeOfQuestion = 1, CorrectValue = true },
               new TrueOrFalseQuestion { Title = "0 is a positive number", GradeOfQuestion = 1, CorrectValue = false },
               new TrueOrFalseQuestion { Title = "Pi is exactly 3", GradeOfQuestion = 1, CorrectValue = false },
               new TrueOrFalseQuestion { Title = "Every square is a rectangle", GradeOfQuestion = 1, CorrectValue = true },
               new TrueOrFalseQuestion { Title = "1/2 equals 0.5", GradeOfQuestion = 1, CorrectValue = true },
               new TrueOrFalseQuestion { Title = "Triangles have 4 sides", GradeOfQuestion = 1, CorrectValue = false },
               new TrueOrFalseQuestion { Title = "Odd numbers are divisible by 2", GradeOfQuestion = 1, CorrectValue = false },
               new TrueOrFalseQuestion { Title = "The sum of angles in a triangle is 180°", GradeOfQuestion = 1, CorrectValue = true }
            };
      
            exam.TrueOrFalseQuestions.AddRange(tfQuestions);

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
