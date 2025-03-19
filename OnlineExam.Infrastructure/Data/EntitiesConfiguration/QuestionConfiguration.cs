namespace OnlineExam.Infrastructure.Data.EntitiesConfiguration
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.ToTable("Questions").HasKey(x => x.Id);

            builder.UseTphMappingStrategy();
        }
    }
}
