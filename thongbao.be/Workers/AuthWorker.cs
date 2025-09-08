using OpenIddict.Abstractions;
using thongbao.be.infrastructure.data;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace thongbao.be.Workers
{
    public class AuthWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuthWorker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<SmDbContext>();
            await context.Database.EnsureCreatedAsync();

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("service-worker") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "service-worker",
                    ClientSecret = "388D45FA-B36B-4988-BA59-B187D329C207",
                    Permissions =
                {
                    Permissions.Endpoints.Token,
                    Permissions.GrantTypes.ClientCredentials
                }
                });
            }

            if (await manager.FindByClientIdAsync("client-web") is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "client-web",
                    ClientSecret = "mBSQUHmZ4be5bQYfhwS7hjJZ2zFOCU2e",
                    RedirectUris =
                    {
                        new Uri("http://localhost:4200/auth/callback")
                    },
                    Permissions =
                    {
                        Permissions.Endpoints.Token,
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Revocation,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.RefreshToken,
                            //Permissions.GrantTypes.ClientCredentials,
                            Permissions.GrantTypes.Password,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Roles,
                    }
                });
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
