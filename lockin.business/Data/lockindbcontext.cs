using lockin.core.models;
using Microsoft.EntityFrameworkCore;


namespace lockin.business.Data
{
    internal class lockindbcontext : DbContext
    {
        public DbSet<Question> Questions
        {
            get; set;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string constring = "Data Source=WIN-PLOQED8CK9Q\\SQLEXPRESS;Initial Catalog=lockin;Integrated Security=True;Persist Security Info=False;Pooling=False;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Application Name=\"SQL Server Management Studio\";Command Timeout=0";
            optionsBuilder.UseSqlServer(constring);
        }
    }
}
