using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task.Entities;

namespace Task.Context
{
    public class dbContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public dbContext(DbContextOptions<dbContext> options) : base(options)
        {

        }
      
    }
}
