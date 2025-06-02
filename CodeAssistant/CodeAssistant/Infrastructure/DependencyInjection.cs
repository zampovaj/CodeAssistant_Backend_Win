using CodeAssistant.Application.UseCases;
using Analyzer.Core.Application.Interfaces;
using Analyzer.Core.Infrastructure.ApplicationServices;
using Analyzer.Core.Infrastructure.Services;
using Analyzer.Core.Domain.Interfaces;
using Analyzer.Core.Infrastructure.Helpers;

namespace CodeAssistant.Infrastructure
{
    /// <summary>
    /// Provides extension method to register infrastructure-level dependencies.
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds services from the Infrastructure layer to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The updated service collection.</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<AnalyzeSolutionUseCase>();
            services.AddScoped<IZipHandler, ZipHandler>();
            services.AddScoped<ISolutionBuilderService, SolutionMSBuilderService>();
            services.AddScoped<ISolutionAnalyzer, RoslynSolutionAnalyzer>();
            services.AddScoped<ISolutionBuilderService, SolutionMSBuilderService>();
            return services;
        }
    }

}
