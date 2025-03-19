namespace OnlineExam.Shared.Interfaces
{
    public interface IAuditableEntity : IEntityCreationTime, IEntityCreatedByUser, IEntityModificationHistory
    {

    }
}
