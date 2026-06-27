using lockin.core.Models;
using Microsoft.EntityFrameworkCore;


namespace lockin.business.Data
{
    public class lockindbcontext : DbContext
    {
        public DbSet<Question> Question
        {
            get; set;
        }

        //  App startup configuration can inject the connection string
        public lockindbcontext(DbContextOptions<lockindbcontext> options) : base(options)
        {

        }

        public DbSet<Topic> Topic { get; set; }
        public DbSet<Answer> Answer {  get; set; }
        public DbSet<UserInfo> UserInfo {  get; set; }
        public DbSet<Location> location { get; set; }



      
    }
}
