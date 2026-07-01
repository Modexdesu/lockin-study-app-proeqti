using lockin.business.Data;
using lockin.business.Repositories;
using lockin.core.Interfaces;
using lockin.wpf.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows;


namespace lockin.wpf
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Build the configuration reader engine
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            // 2. Initialize Dependency Injection Container
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // 3. Launch the App
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // 1. Extract the connection string from our appsettings.json file
            string connectionString = Configuration.GetConnectionString("DefaultConnection");

            // 2. Register Database Context with TRANSIENT lifetimes
            // By default, AddDbContext uses Scoped. We override BOTH the context lifetime 
            // and the options lifetime to Transient so a new one is spawned and killed on demand.
            services.AddDbContext<lockindbcontext>(options =>
                options.UseSqlServer(connectionString),
                ServiceLifetime.Transient,
                ServiceLifetime.Transient);

            // 3. Convert all Engine Repositories to Transient
            // This forces the DI container to build a fresh repository with a fresh DB connection 
            // every single time a ViewModel asks for it.
            services.AddTransient<IQuestionRepository, QuestionRepository>();
            services.AddTransient<IAnswerRepository, AnswerRepository>();
            services.AddTransient<ITopicRepository, TopicRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            // 4. UI Layer components remain Transient
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();
        }
    }
}