using System.ComponentModel.DataAnnotations;

namespace Task.Models
{
    public class ChangePasswordVM
    {
        [Required, MaxLength(16), DataType(DataType.Password)]
        public string Password { get; set; }
        [Required, MaxLength(16), DataType(DataType.Password), Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        public string Token { get; set; }
    }
}
