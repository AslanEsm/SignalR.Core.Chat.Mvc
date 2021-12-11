using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR.Core.Chat.Mvc.Models;

namespace SignalR.Core.Chat.Mvc.Data
{
    public class ApplicationDbContext : IdentityDbContext<ChatUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region Dbsets


        public DbSet<Message> Messages { get; set; }


        #endregion


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
