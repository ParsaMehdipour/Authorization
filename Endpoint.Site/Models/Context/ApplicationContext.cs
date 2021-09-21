using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Endpoint.Site.Models.Context
{
    public class ApplicationContext : IdentityDbContext
    {

        public DbSet<Car> Cars { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            :base(options)
        {
            
        }

    }
}
