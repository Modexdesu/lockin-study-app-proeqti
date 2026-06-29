using lockin.core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;


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
            Database.EnsureCreated();

        }

        public DbSet<Topic> Topic { get; set; }
        public DbSet<UserInfo> UserInfo {  get; set; }
        public DbSet<Location> Location { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Hardcode Topics
            modelBuilder.Entity<Topic>().HasData(
                new Topic { TopicId = 1, TopicName = "C# Programming" },
                new Topic { TopicId = 2, TopicName = "General History" }
            );

            // 2. Hardcode Questions linked to those Topics using your exact property names
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    QuestionId = 1,
                    TopicId = 1,
                    QuestionText = "What data type is used for whole numbers?",
                    CorrectAnswer = "int"
                },
                new Question
                {
                    QuestionId = 2,
                    TopicId = 1,
                    QuestionText = "What does EF stand for?",
                    CorrectAnswer = "Entity Framework"
                },
                new Question
                {
                    QuestionId = 3,
                    TopicId = 2,
                    QuestionText = "In what year did the Titanic sink?",
                    CorrectAnswer = "1912"
                }
            );
        }

    }
}
