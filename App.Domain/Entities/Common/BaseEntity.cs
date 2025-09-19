namespace App.Domain.Entities.Common
{
    public class BaseEntity<TId>
    {
        public TId Id { get; set; } = default!;
    }
}
