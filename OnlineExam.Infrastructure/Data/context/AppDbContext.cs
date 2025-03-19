namespace OnlineExam.Infrastructure.Data.context
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<StudentExam> StudentExams { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ChooseQuestion> ChooseQuestions { get; set; }
        public DbSet<TrueOrFalseQuestion> TrueOrFalseQuestions { get; set; }
        public DbSet<Choice> Choices { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;
        public AppDbContext(DbContextOptions<AppDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Ignore<BaseEntity>();

            builder.Entity<AppUser>().ToTable("Users", schema: "Security");
            builder.Entity<IdentityRole>().ToTable("Roles", schema: "Security");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles", schema: "Security");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims", schema: "Security");

            var cascadeFKs = builder.Model.GetEntityTypes()
                                               .SelectMany(t => t.GetForeignKeys())
                                               .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries<BaseEntity>();

            var CurrentUserIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);

            int? CurrentUserId = null;
            if (CurrentUserIdClaim != null && int.TryParse(CurrentUserIdClaim.Value, out var parsedUserId))
            {
                CurrentUserId = parsedUserId;
            }

            foreach (var entryEntity in entries)
            {
                if (entryEntity != null && CurrentUserId is not null)
                {
                    if (entryEntity.State == EntityState.Added)
                    {
                        entryEntity.Property(x => x.CreatedAt).CurrentValue = DateTime.UtcNow;
                        entryEntity.Property(x => x.CreatedByUserId).CurrentValue = CurrentUserId.Value;
                    }
                    else if (entryEntity.State == EntityState.Modified)
                    {
                        if (entryEntity.Properties.Any(p => p.IsModified)) //  Only update if properties are modified
                        {
                            if (entryEntity.Property(x => x.FirstUpdatedTime).CurrentValue is null &&
                            entryEntity.Property(x => x.FirstUpdatedByUserId).CurrentValue is null)
                            {
                                entryEntity.Property(x => x.FirstUpdatedByUserId).CurrentValue = CurrentUserId.Value;
                                entryEntity.Property(x => x.FirstUpdatedTime).CurrentValue = DateTime.UtcNow;
                            }
                            else
                            {
                                entryEntity.Property(x => x.LastUpdatedByUserId).CurrentValue = CurrentUserId.Value;
                                entryEntity.Property(x => x.LastUpdatedTime).CurrentValue = DateTime.UtcNow;
                            }
                        }
                    }
                    else if (entryEntity.State == EntityState.Deleted && entryEntity.Entity is ISoftDeletable)
                    {
                        entryEntity.State = EntityState.Modified;
                        entryEntity.Entity.MarkAsDeleted(CurrentUserId.Value);
                    }
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
