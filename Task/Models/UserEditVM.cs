using System.ComponentModel.DataAnnotations;
using Task.Entities;

namespace Task.Models
{
    public class UserEditVM
    {

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please enter an email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        public List<AppRole> AllRoles { get; set; } = new List<AppRole>();
        public List<string> SelectedRoles { get; set; } 
    }
}
