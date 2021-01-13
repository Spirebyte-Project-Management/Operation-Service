using Convey;
using Convey.Logging;
using Convey.Secrets.Vault;
using Convey.Types;
using Convey.WebApi;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Spirebyte.Services.Operations.Application.Queries;
using Spirebyte.Services.Operations.Application.Services.Interfaces;
using Spirebyte.Services.Operations.Infrastructure;
using Spirebyte.Services.Operations.Infrastructure.Hubs;
using Spirebyte.Services.Operations.Infrastructure.Services;
using System.Threading.Tasks;

namespace Spirebyte.Services.Operations.API
{
    public class Program
    {
        public static async Task Main(string[] args)
            => await CreateWebHostBuilder(args)
                .Build()
                .RunAsync();

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
            => WebHost.CreateDefaultBuilder(args)
                .ConfigureServices(services => services
                    .AddCors()
                    .AddConvey()
                    .AddWebApi()
                    .AddInfrastructure()
                    .Build())
                .Configure(app => app
                    .UseCors(x => x
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .SetIsOriginAllowed(origin => true)
                        .AllowCredentials())
                    .UseInfrastructure()
                    .UseEndpoints(endpoints => endpoints
                        .Get("", ctx => ctx.Response.WriteAsync(ctx.RequestServices.GetService<AppOptions>().Name))
                        .Get<GetOperation>("operations/{operationId}", async (query, ctx) =>
                        {
                            var operation = await ctx.RequestServices.GetService<IOperationsService>()
                                .GetAsync(query.OperationId);
                            if (operation is null)
                            {
                                await ctx.Response.NotFound();
                                return;
                            }

                            await ctx.Response.WriteJsonAsync(operation);
                        }))
                    .UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<SpirebyteHub>("/spirebyte");
                        endpoints.MapGrpcService<GrpcServiceHost>();
                    }))
                .UseLogging()
                .UseVault();
    }
}
