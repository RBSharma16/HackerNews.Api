using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace HackerNews.Test.Program
{
    /// <summary>
    /// Program class fixture.
    /// </summary>
    public class ProgramFixture
    {
        public ProgramFixture() 
        {
            services = new ServiceCollection();

            var hostEnvironment = new HostingEnvironment 
            { 
                EnvironmentName = "Development",
                ApplicationName = string.Empty,
                ContentRootPath = string.Empty,
                WebRootPath = string.Empty,
                ContentRootFileProvider = null,
                WebRootFileProvider = null
            };
            services.AddSingleton<IHostEnvironment>(hostEnvironment);
            services.AddSingleton<IWebHostEnvironment>(hostEnvironment);

            var configuration = new ConfigurationBuilder().Build();
            services.AddSingleton<IConfiguration>(configuration);

            builder = WebApplication.CreateBuilder(new string[0]);
            builder.Environment.EnvironmentName = "Development";
        }
        public ServiceCollection services { get; set; }
        public WebApplicationBuilder builder { get; set; }
    }
}