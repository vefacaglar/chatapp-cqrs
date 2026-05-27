namespace ChatApp.Domain
{
    public abstract class BaseEntity<T>
    {
        public BaseEntity()
        {
            CreatedAt = DateTime.UtcNow;
        }

        public T? Id { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
