using HackersNews.Service;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using _program = HackersNews.Api.Program;

namespace HackerNews.Test.Program
{
    /// <summary>
    /// Programclass tests
    /// </summary>
    public class ProgramTests : IClassFixture<ProgramFixture>
    {
        private readonly ProgramFixture _programFixture;

        public ProgramTests(ProgramFixture programFixture) 
        {
            _programFixture = programFixture;
        }

        [Fact]
        public void ConfigureServices_RegistersServicesCorrectly()
        {
            // Arrange
            _programFixture.services.AddSingleton<IActionDescriptorCollectionProvider>(new ActionDescriptorCollectionProviderMock());

            // Act
            _program.ConfigureServices(_programFixture.services);

            // Assert
            var serviceProvider = _programFixture.services.BuildServiceProvider();
            Assert.NotNull(serviceProvider.GetService<ILoggerFactory>());
            Assert.NotNull(serviceProvider.GetService<IMemoryCache>());
            Assert.NotNull(serviceProvider.GetService<IHttpClientFactory>());
            Assert.NotNull(serviceProvider.GetService<ISwaggerProvider>());
            Assert.NotNull(serviceProvider.GetService<IHackersNewsApiClient>());
        }

        [Fact]
        public void ConfigureMiddleware_DevelopmentEnvironment_ConfiguresSwagger()
        {
            // Arrange
            _program.ConfigureServices(_programFixture.builder.Services);
            var app = _programFixture.builder.Build();

            // Act
            _program.ConfigureMiddleware(app);
            var endpointDataSource = app.Services.GetService<EndpointDataSource>();

            // Assert
            Assert.NotNull(endpointDataSource);
            Assert.NotNull(app.Services.GetService<ICorsService>());
        }
    }
}
