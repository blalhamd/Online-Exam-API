namespace OnlineExam.Shared.Interfaces
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; } 
        DateTimeOffset? DeletedTime { get; set; }
        int? DeletedByUserId { get; set; }
    }
}
