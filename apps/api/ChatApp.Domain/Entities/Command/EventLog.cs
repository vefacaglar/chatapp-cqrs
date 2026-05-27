using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Domain.Entities.Command
{
    public class EventLog : BaseEntity<long>
    {
        public EventLog()
        {
            CreatedAt = DateTime.UtcNow;
        }

        [MaxLength(200)]
        public string Type { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        public string Payload { get; set; } = string.Empty;
    }
}
