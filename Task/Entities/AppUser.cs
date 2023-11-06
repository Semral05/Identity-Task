using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public string? ImagePath { get; set; }
        public string Gender { get; set; }
  

    }
}
