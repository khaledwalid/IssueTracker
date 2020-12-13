using System.Collections.Generic;
using IdentityServer4.Models;

namespace IssueTracker.Common.Auth
{
    public static class IdentityConfig
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope("api1", "Full access to API #1") // "full access" scope
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("api1", "API #1") {Scopes = {"api1"}}
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                // Swashbuckle & NSwag
                new Client
                {
                    ClientId = "demo_api_swagger",
                    ClientName = "Swagger UI for demo_api",
                    ClientSecrets = {new Secret("secret".Sha256())}, // change me!
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    RedirectUris = {"http://localhost:5000/swagger/oauth2-redirect.html"},
                    AllowedCorsOrigins = {"http://localhost:5000"},
                    AllowedScopes = {"api1"}
                }
            };
    }
}