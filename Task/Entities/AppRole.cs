using Microsoft.AspNetCore.Identity;

namespace Task.Entities
{
    public class AppRole : IdentityRole<int>
    {
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
