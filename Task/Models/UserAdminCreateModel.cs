using System.ComponentModel.DataAnnotations;

namespace Task.Models
{
    public class UserAdminCreateModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter an email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }
    }
}
