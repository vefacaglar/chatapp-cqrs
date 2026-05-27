using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Chat.Request
{
    public class SendMessageRequest : IValidatableObject
    {
        private string _userName = string.Empty;
        private string _message = string.Empty;

        public Guid RoomId { get; set; }

        public string UserName
        {
            get => _userName.Trim();
            set => _userName = value;
        }

        public string Message
        {
            get => _message.Trim();
            set => _message = value;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RoomId == Guid.Empty)
            {
                yield return new ValidationResult("RoomId is required.", new[] { nameof(RoomId) });
            }

            if (string.IsNullOrWhiteSpace(UserName))
            {
                yield return new ValidationResult("UserName is required.", new[] { nameof(UserName) });
            }

            if (string.IsNullOrWhiteSpace(Message))
            {
                yield return new ValidationResult("Message is required.", new[] { nameof(Message) });
            }
        }
    }
}
