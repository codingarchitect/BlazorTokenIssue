// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityService
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("webapi"),
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new ApiResource("webapi")
                {
                    Scopes =
                    {
                        "webapi"
                    },
                },
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client()
                {
                    ClientId = "blazor-app",
                    ClientName = "Blazor App",
                    ClientSecrets =
                    {
                        new Secret("244c6179-9650-4f99-be2b-e2265cccef77".Sha256()),
                    },
                    RedirectUris =
                    {
                        "https://localhost:7055/signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "https://localhost:7055/signout-callback-oidc"
                    },
                    AllowedGrantTypes = GrantTypes.Code,
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        "openid",
                        "profile",
                        "webapi",
                    },
                    AccessTokenLifetime = (int)TimeSpan.FromDays(7).TotalSeconds,
                }
            };
    }
}
