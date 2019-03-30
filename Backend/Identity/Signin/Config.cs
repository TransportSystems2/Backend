using IdentityModel;
using IdentityServer4.Models;
using System.Collections.Generic;
using TransportSystems.Backend.Identity.Signin.Constants;
using static IdentityServer4.IdentityServerConstants;

namespace TransportSystems.Backend.Identity.Signin
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Phone(),
                new IdentityResource()
                {
                    Name = "custom.profile",
                    UserClaims =
                    {
                        JwtClaimTypes.Role,
                        JwtClaimTypes.PhoneNumber
                    }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TSAPI", "TransportSystems API")
                {
                    ApiSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    UserClaims = 
                    {
                        JwtClaimTypes.Role,
                        JwtClaimTypes.PhoneNumber
                    }
                },
                new ApiResource ("myapi", "My Api")
                {
                    UserClaims =
                    {
                        "role",
                        JwtClaimTypes.Role,
                        JwtClaimTypes.PhoneNumber
                    }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "IdentitySignup",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = { "Identity" }
                },

                new Client
                {
                    ClientId = "TSAPI",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = { "TSAPI" }
                },

                new Client
                {
                    ClientName = "phone_number_authentication",
                    ClientId = "phone_number_authentication",
                    AllowedGrantTypes = 
                    {
                        AuthConstants.GrantType.PhoneNumberToken,
                        GrantType.Hybrid,
                    },

                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AlwaysSendClientClaims = true,

                    AllowedScopes =
                    {
                        StandardScopes.OpenId,
                        StandardScopes.OfflineAccess,
                        "myapi",
                        "TSAPI",
                        "custom.profile"
                    },

                    RedirectUris = 
                    {
                        "http://localhost:82"
                    },

                    AllowOfflineAccess = true
                }
            };
        }
    }
}