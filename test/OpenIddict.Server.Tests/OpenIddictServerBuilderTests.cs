﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace OpenIddict.Server.Tests
{
    public class OpenIddictServerBuilderTests
    {
        [Fact]
        public void Configure_OptionsAreCorrectlyAmended()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.Configure(configuration => configuration.Description.DisplayName = "OpenIddict");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("OpenIddict", options.Value.Description.DisplayName);
        }

        [Fact]
        public void AddEphemeralSigningKey_SigningKeyIsCorrectlyAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AddEphemeralSigningKey();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(1, options.Value.SigningCredentials.Count);
        }

        [Theory]
        [InlineData(SecurityAlgorithms.RsaSha256Signature)]
        [InlineData(SecurityAlgorithms.RsaSha384Signature)]
        [InlineData(SecurityAlgorithms.RsaSha512Signature)]
#if SUPPORTS_ECDSA
        [InlineData(SecurityAlgorithms.EcdsaSha256Signature)]
        [InlineData(SecurityAlgorithms.EcdsaSha384Signature)]
        [InlineData(SecurityAlgorithms.EcdsaSha512Signature)]
#endif
        public void AddEphemeralSigningKey_SigningCredentialsUseSpecifiedAlgorithm(string algorithm)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AddEphemeralSigningKey(algorithm);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();
            var credentials = options.Value.SigningCredentials[0];

            // Assert
            Assert.Equal(algorithm, credentials.Algorithm);
        }

        [Theory]
        [InlineData(SecurityAlgorithms.HmacSha256Signature)]
        [InlineData(SecurityAlgorithms.RsaSha256Signature)]
#if SUPPORTS_ECDSA
        [InlineData(SecurityAlgorithms.EcdsaSha256Signature)]
        [InlineData(SecurityAlgorithms.EcdsaSha384Signature)]
        [InlineData(SecurityAlgorithms.EcdsaSha512Signature)]
#endif
        public void AddSigningKey_SigningKeyIsCorrectlyAdded(string algorithm)
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            var factory = Mock.Of<CryptoProviderFactory>(mock =>
                mock.IsSupportedAlgorithm(algorithm, It.IsAny<SecurityKey>()));

            var key = Mock.Of<SecurityKey>(mock => mock.CryptoProviderFactory == factory);

            // Act
            builder.AddSigningKey(key);

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Same(key, options.Value.SigningCredentials[0].Key);
        }

        [Fact]
        public void AddSigningCertificate_SigningKeyIsCorrectlyAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AddSigningCertificate(
                assembly: typeof(OpenIddictServerBuilderTests).GetTypeInfo().Assembly,
                resource: "OpenIddict.Server.Tests.Certificate.pfx",
                password: "OpenIddict");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.IsType(typeof(X509SecurityKey), options.Value.SigningCredentials[0].Key);
        }

        [Fact]
        public void AllowAuthorizationCodeFlow_CodeFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowAuthorizationCodeFlow();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains(OpenIdConnectConstants.GrantTypes.AuthorizationCode, options.Value.GrantTypes);
        }

        [Fact]
        public void AllowClientCredentialsFlow_ClientCredentialsFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowClientCredentialsFlow();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains(OpenIdConnectConstants.GrantTypes.ClientCredentials, options.Value.GrantTypes);
        }

        [Fact]
        public void AllowCustomFlow_CustomFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowCustomFlow("urn:ietf:params:oauth:grant-type:custom_grant");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains("urn:ietf:params:oauth:grant-type:custom_grant", options.Value.GrantTypes);
        }

        [Fact]
        public void AllowImplicitFlow_ImplicitFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowImplicitFlow();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains(OpenIdConnectConstants.GrantTypes.Implicit, options.Value.GrantTypes);
        }

        [Fact]
        public void AllowPasswordFlow_PasswordFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowPasswordFlow();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains(OpenIdConnectConstants.GrantTypes.Password, options.Value.GrantTypes);
        }

        [Fact]
        public void AllowRefreshTokenFlow_RefreshTokenFlowIsAddedToGrantTypes()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.AllowRefreshTokenFlow();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains(OpenIdConnectConstants.GrantTypes.RefreshToken, options.Value.GrantTypes);
        }

        [Fact]
        public void DisableConfigurationEndpoint_ConfigurationEndpointIsDisabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.DisableConfigurationEndpoint();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(PathString.Empty, options.Value.ConfigurationEndpointPath);
        }

        [Fact]
        public void DisableCryptographyEndpoint_CryptographyEndpointIsDisabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.DisableCryptographyEndpoint();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(PathString.Empty, options.Value.CryptographyEndpointPath);
        }

        [Fact]
        public void DisableSlidingExpiration_SlidingExpirationIsDisabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.DisableSlidingExpiration();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.False(options.Value.UseSlidingExpiration);
        }

        [Fact]
        public void DisableTokenRevocation_TokenRevocationIsDisabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.DisableTokenRevocation();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.True(options.Value.DisableTokenRevocation);
        }

        [Fact]
        public void EnableAuthorizationEndpoint_AuthorizationEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableAuthorizationEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.AuthorizationEndpointPath);
        }

        [Fact]
        public void EnableIntrospectionEndpoint_IntrospectionEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableIntrospectionEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.IntrospectionEndpointPath);
        }

        [Fact]
        public void EnableLogoutEndpoint_LogoutEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableLogoutEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.LogoutEndpointPath);
        }

        [Fact]
        public void EnableRequestCaching_RequestCachingIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableRequestCaching();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.True(options.Value.EnableRequestCaching);
        }

        [Fact]
        public void EnableRevocationEndpoint_RevocationEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableRevocationEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.RevocationEndpointPath);
        }

        [Fact]
        public void EnableScopeValidation_ScopeValidationIsDisabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableScopeValidation();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.True(options.Value.EnableScopeValidation);
        }

        [Fact]
        public void EnableTokenEndpoint_TokenEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableTokenEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.TokenEndpointPath);
        }

        [Fact]
        public void EnableUserinfoEndpoint_UserinfoEndpointIsEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.EnableUserinfoEndpoint("/endpoint-path");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal("/endpoint-path", options.Value.UserinfoEndpointPath);
        }

        [Fact]
        public void RequireClientIdentification_ClientIdentificationIsEnforced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.RequireClientIdentification();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.True(options.Value.RequireClientIdentification);
        }

        [Fact]
        public void SetAccessTokenLifetime_DefaultAccessTokenLifetimeIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.SetAccessTokenLifetime(TimeSpan.FromMinutes(42));

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(42), options.Value.AccessTokenLifetime);
        }

        [Fact]
        public void SetAuthorizationCodeLifetime_DefaultAuthorizationCodeLifetimeIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.SetAuthorizationCodeLifetime(TimeSpan.FromMinutes(42));

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(42), options.Value.AuthorizationCodeLifetime);
        }

        [Fact]
        public void SetIdentityTokenLifetime_DefaultIdentityTokenLifetimeIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.SetIdentityTokenLifetime(TimeSpan.FromMinutes(42));

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(42), options.Value.IdentityTokenLifetime);
        }

        [Fact]
        public void SetRefreshTokenLifetime_DefaultRefreshTokenLifetimeIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.SetRefreshTokenLifetime(TimeSpan.FromMinutes(42));

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(TimeSpan.FromMinutes(42), options.Value.RefreshTokenLifetime);
        }

        [Fact]
        public void SetIssuer_AddressIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.SetIssuer(new Uri("http://www.fabrikam.com/"));

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Equal(new Uri("http://www.fabrikam.com/"), options.Value.Issuer);
        }

        [Fact]
        public void RegisterClaims_ClaimsAreAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.RegisterClaims("custom_claim_1", "custom_claim_2");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains("custom_claim_1", options.Value.Claims);
            Assert.Contains("custom_claim_2", options.Value.Claims);
        }

        [Fact]
        public void RegisterScopes_ScopesAreAdded()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.RegisterScopes("custom_scope_1", "custom_scope_2");

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.Contains("custom_scope_1", options.Value.Scopes);
            Assert.Contains("custom_scope_2", options.Value.Scopes);
        }

        [Fact]
        public void UseDataProtectionProvider_DefaultProviderIsReplaced()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.UseDataProtectionProvider(new EphemeralDataProtectionProvider());

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.IsType(typeof(EphemeralDataProtectionProvider), options.Value.DataProtectionProvider);
        }

        [Fact]
        public void UseJsonWebTokens_AccessTokenHandlerIsCorrectlySet()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.UseJsonWebTokens();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.IsType<JwtSecurityTokenHandler>(options.Value.AccessTokenHandler);
        }

        [Fact]
        public void UseReferenceTokens_ReferenceTokensAreEnabled()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddOptions();

            var builder = CreateBuilder(services);

            // Act
            builder.UseReferenceTokens();

            var provider = services.BuildServiceProvider();
            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();

            // Assert
            Assert.True(options.Value.UseReferenceTokens);
        }

        private static OpenIddictServerBuilder CreateBuilder(IServiceCollection services)
            => services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.SetDefaultApplicationEntity<OpenIddictApplication>()
                           .SetDefaultAuthorizationEntity<OpenIddictAuthorization>()
                           .SetDefaultScopeEntity<OpenIddictScope>()
                           .SetDefaultTokenEntity<OpenIddictToken>();
                })

                .AddServer();

        private static IServiceCollection CreateServices()
        {
            var services = new ServiceCollection();
            services.AddAuthentication();
            services.AddDistributedMemoryCache();
            services.AddLogging();
            services.AddSingleton<IHostingEnvironment, HostingEnvironment>();

            return services;
        }

        private static OpenIddictServerOptions GetOptions(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<IOptions<OpenIddictServerOptions>>();
            return options.Value;
        }

        public class OpenIddictApplication { }
        public class OpenIddictAuthorization { }
        public class OpenIddictScope { }
        public class OpenIddictToken { }
    }
}
