using System.ComponentModel.DataAnnotations;

namespace ChatApp.Domain.Chat.Request
{
    public class CreateChatRoomRequest : IValidatableObject
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name.Trim();
            set => _name = value;
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                yield return new ValidationResult("Name is required.", new[] { nameof(Name) });
            }
            else if (Name.Length > 50)
            {
                yield return new ValidationResult("Name must be 50 characters or fewer.", new[] { nameof(Name) });
            }
        }
    }
}
