using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityExample.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> option
        ) : base(option)
        {
        }
    }
}