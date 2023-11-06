using System.ComponentModel.DataAnnotations;

namespace Task.Models
{
    public class UserSignInModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password field required")]
        public string Password { get; set; }  
        
        public bool RememberMe { get; set; }    

        public string? ReturnUrl { get; set; }

        
    }
}
