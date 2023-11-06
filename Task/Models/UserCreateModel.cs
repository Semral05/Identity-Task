using System.ComponentModel.DataAnnotations;

namespace Task.Models
{
    public class UserCreateModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Please enter an email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password field required")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage = "password not appropriate")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
    }
}
