using Microsoft.EntityFrameworkCore;

namespace MessageApp.Models
{
    public class MessageAppDBContext : DbContext 
    {
        public DbSet<Message> Message { get; set; }
        public DbSet<MessageResult> MessageResult { get; set; }
        public DbSet<MessageHistory> MessageHistory { get; set; }
        public DbSet<SMSConfiguration> SMSConfiguration { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=MSI\SQLEXPRESS;Initial Catalog=MessageAppDB;Integrated Security=True");
        }
    }

   
}
