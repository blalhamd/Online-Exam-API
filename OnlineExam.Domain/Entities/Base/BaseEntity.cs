namespace OnlineExam.Domain.Entities
{
    public class BaseEntity : IAuditableEntity, ISoftDeletable
    {
        public int Id { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTimeOffset? FirstUpdatedTime { get; set; }
        public DateTimeOffset? LastUpdatedTime { get; set; }
        public int? FirstUpdatedByUserId { get; set; }
        public int? LastUpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedTime { get; set; }
        public int? DeletedByUserId { get; set; }

        public void MarkAsDeleted(int userId)
        {
            IsDeleted = true;
            DeletedByUserId = userId;
            DeletedTime = DateTimeOffset.UtcNow;
        }
    }
}
