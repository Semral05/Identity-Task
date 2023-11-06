using System.ComponentModel.DataAnnotations;

namespace Task.Models
{
    public class ForgotPasswordVM
    {
        [Required, MaxLength(255), DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
