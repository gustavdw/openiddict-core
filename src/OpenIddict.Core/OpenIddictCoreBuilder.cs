﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using System;
using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure the OpenIddict core services.
    /// </summary>
    public class OpenIddictCoreBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OpenIddictCoreBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public OpenIddictCoreBuilder([NotNull] IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            Services = services;
        }

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default OpenIddict core configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder Configure([NotNull] Action<OpenIddictCoreOptions> configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Adds a custom application store by a custom implementation derived
        /// from <see cref="IOpenIddictApplicationStore{TApplication}"/>.
        /// Note: when using this overload, the application store
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TStore">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddApplicationStore<TStore>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TStore : class
            => AddApplicationStore(typeof(TStore), lifetime);

        /// <summary>
        /// Adds a custom application store by a custom implementation derived
        /// from <see cref="IOpenIddictApplicationStore{TApplication}"/>.
        /// Note: when using this overload, the application store can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddApplicationStore(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(IOpenIddictApplicationStore<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictApplicationStore<>)
            // or closed generics (e.g OpenIddictApplicationStore<OpenIddictApplication>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictApplicationStore<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictApplicationStore<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Adds a custom authorization store by a custom implementation derived
        /// from <see cref="IOpenIddictAuthorizationStore{TAuthorization}"/>.
        /// Note: when using this overload, the authorization store
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TStore">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddAuthorizationStore<TStore>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TStore : class
            => AddAuthorizationStore(typeof(TStore), lifetime);

        /// <summary>
        /// Adds a custom authorization store by a custom implementation derived
        /// from <see cref="IOpenIddictAuthorizationStore{TAuthorization}"/>.
        /// Note: when using this overload, the authorization store can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddAuthorizationStore(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(IOpenIddictAuthorizationStore<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictAuthorizationStore<>)
            // or closed generics (e.g OpenIddictAuthorizationStore<OpenIddictAuthorization>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictAuthorizationStore<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictAuthorizationStore<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Adds a custom scope store by a custom implementation derived
        /// from <see cref="IOpenIddictScopeStore{TScope}"/>.
        /// Note: when using this overload, the scope store
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TStore">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddScopeStore<TStore>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TStore : class
            => AddScopeStore(typeof(TStore), lifetime);

        /// <summary>
        /// Adds a custom scope store by a custom implementation derived
        /// from <see cref="IOpenIddictScopeStore{TScope}"/>.
        /// Note: when using this overload, the scope store can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddScopeStore(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(IOpenIddictScopeStore<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictScopeStore<>)
            // or closed generics (e.g OpenIddictScopeStore<OpenIddictScope>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictScopeStore<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictScopeStore<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Adds a custom token store by a custom implementation derived
        /// from <see cref="IOpenIddictTokenStore{TToken}"/>.
        /// Note: when using this overload, the token store
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TStore">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddTokenStore<TStore>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TStore : class
            => AddTokenStore(typeof(TStore), lifetime);

        /// <summary>
        /// Adds a custom token store by a custom implementation derived
        /// from <see cref="IOpenIddictTokenStore{TToken}"/>.
        /// Note: when using this overload, the token store can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder AddTokenStore(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(IOpenIddictTokenStore<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictTokenStore<>)
            // or closed generics (e.g OpenIddictTokenStore<OpenIddictToken>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictTokenStore<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(IOpenIddictTokenStore<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Replace the default application manager by a custom manager derived
        /// from <see cref="OpenIddictApplicationManager{TApplication}"/>.
        /// Note: when using this overload, the application manager
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TManager">The type of the custom manager.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceApplicationManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TManager : class
            => ReplaceApplicationManager(typeof(TManager), lifetime);

        /// <summary>
        /// Replace the default application manager by a custom manager derived
        /// from <see cref="OpenIddictApplicationManager{TApplication}"/>.
        /// Note: when using this overload, the application manager can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom manager.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceApplicationManager(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(OpenIddictApplicationManager<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictApplicationManager<>)
            // or closed generics (e.g OpenIddictApplicationManager<OpenIddictApplication>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(OpenIddictApplicationManager<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(OpenIddictApplicationManager<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Replaces the default application store resolver by a custom implementation.
        /// </summary>
        /// <typeparam name="TResolver">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceApplicationStoreResolver<TResolver>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TResolver : IOpenIddictApplicationStoreResolver
            => ReplaceApplicationStoreResolver(typeof(TResolver), lifetime);

        /// <summary>
        /// Replaces the default application store resolver by a custom implementation.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceApplicationStoreResolver(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IOpenIddictApplicationStoreResolver).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            Services.Replace(new ServiceDescriptor(typeof(IOpenIddictApplicationStoreResolver), type, lifetime));

            return this;
        }

        /// <summary>
        /// Replace the default authorization manager by a custom manager derived
        /// from <see cref="OpenIddictAuthorizationManager{TAuthorization}"/>.
        /// Note: when using this overload, the authorization manager
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TManager">The type of the custom manager.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceAuthorizationManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TManager : class
            => ReplaceAuthorizationManager(typeof(TManager), lifetime);

        /// <summary>
        /// Replace the default authorization manager by a custom manager derived
        /// from <see cref="OpenIddictAuthorizationManager{TAuthorization}"/>.
        /// Note: when using this overload, the authorization manager can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom manager.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceAuthorizationManager(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(OpenIddictAuthorizationManager<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictAuthorizationManager<>)
            // or closed generics (e.g OpenIddictAuthorizationManager<OpenIddictAuthorization>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(OpenIddictAuthorizationManager<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(OpenIddictAuthorizationManager<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Replaces the default authorization store resolver by a custom implementation.
        /// </summary>
        /// <typeparam name="TResolver">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceAuthorizationStoreResolver<TResolver>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TResolver : IOpenIddictAuthorizationStoreResolver
            => ReplaceAuthorizationStoreResolver(typeof(TResolver), lifetime);

        /// <summary>
        /// Replaces the default authorization store resolver by a custom implementation.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceAuthorizationStoreResolver(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IOpenIddictAuthorizationStoreResolver).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            Services.Replace(new ServiceDescriptor(typeof(IOpenIddictAuthorizationStoreResolver), type, lifetime));

            return this;
        }

        /// <summary>
        /// Replace the default scope manager by a custom manager
        /// derived from <see cref="OpenIddictScopeManager{TScope}"/>.
        /// Note: when using this overload, the scope manager
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TManager">The type of the custom manager.</typeparam>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceScopeManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TManager : class
            => ReplaceScopeManager(typeof(TManager), lifetime);

        /// <summary>
        /// Replace the default scope manager by a custom manager
        /// derived from <see cref="OpenIddictScopeManager{TScope}"/>.
        /// Note: when using this overload, the scope manager can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom manager.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceScopeManager(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(OpenIddictScopeManager<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictScopeManager<>)
            // or closed generics (e.g OpenIddictScopeManager<OpenIddictScope>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(OpenIddictScopeManager<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(OpenIddictScopeManager<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Replaces the default scope store resolver by a custom implementation.
        /// </summary>
        /// <typeparam name="TResolver">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceScopeStoreResolver<TResolver>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TResolver : IOpenIddictScopeStoreResolver
            => ReplaceScopeStoreResolver(typeof(TResolver), lifetime);

        /// <summary>
        /// Replaces the default scope store resolver by a custom implementation.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceScopeStoreResolver(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IOpenIddictScopeStoreResolver).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            Services.Replace(new ServiceDescriptor(typeof(IOpenIddictScopeStoreResolver), type, lifetime));

            return this;
        }

        /// <summary>
        /// Replace the default token manager by a custom manager
        /// derived from <see cref="OpenIddictTokenManager{TToken}"/>.
        /// Note: when using this overload, the token manager
        /// must be either a non-generic or closed generic service.
        /// </summary>
        /// <typeparam name="TManager">The type of the custom manager.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceTokenManager<TManager>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TManager : class
            => ReplaceTokenManager(typeof(TManager), lifetime);

        /// <summary>
        /// Replace the default token manager by a custom manager
        /// derived from <see cref="OpenIddictTokenManager{TToken}"/>.
        /// Note: when using this overload, the token manager can be
        /// either a non-generic, a closed or an open generic service.
        /// </summary>
        /// <param name="type">The type of the custom manager.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceTokenManager(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var root = OpenIddictCoreHelpers.FindGenericBaseType(type, typeof(OpenIddictTokenManager<>));
            if (root == null)
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            // Note: managers can be either open generics (e.g OpenIddictTokenManager<>)
            // or closed generics (e.g OpenIddictTokenManager<OpenIddictToken>).
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (type.GetGenericArguments().Length != 1)
                {
                    throw new ArgumentException("The specified type is invalid.", nameof(type));
                }

                Services.Replace(new ServiceDescriptor(typeof(OpenIddictTokenManager<>), type, lifetime));
            }

            else
            {
                Services.Replace(new ServiceDescriptor(typeof(OpenIddictTokenManager<>)
                    .MakeGenericType(root.GenericTypeArguments[0]), type, lifetime));
            }

            return this;
        }

        /// <summary>
        /// Replaces the default token store resolver by a custom implementation.
        /// </summary>
        /// <typeparam name="TResolver">The type of the custom store.</typeparam>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceTokenStoreResolver<TResolver>(ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TResolver : IOpenIddictTokenStoreResolver
            => ReplaceTokenStoreResolver(typeof(TResolver), lifetime);

        /// <summary>
        /// Replaces the default token store resolver by a custom implementation.
        /// </summary>
        /// <param name="type">The type of the custom store.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder ReplaceTokenStoreResolver(
            [NotNull] Type type, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (!typeof(IOpenIddictTokenStoreResolver).IsAssignableFrom(type))
            {
                throw new ArgumentException("The specified type is invalid.", nameof(type));
            }

            Services.Replace(new ServiceDescriptor(typeof(IOpenIddictTokenStoreResolver), type, lifetime));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default application entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder SetDefaultApplicationEntity<TApplication>()
            where TApplication : class, new()
            => Configure(options => options.DefaultApplicationType = typeof(TApplication));

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default authorization entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder SetDefaultAuthorizationEntity<TAuthorization>()
            where TAuthorization : class, new()
            => Configure(options => options.DefaultAuthorizationType = typeof(TAuthorization));

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default scope entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder SetDefaultScopeEntity<TScope>()
            where TScope : class, new()
            => Configure(options => options.DefaultScopeType = typeof(TScope));

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default token entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public OpenIddictCoreBuilder SetDefaultTokenEntity<TToken>()
            where TToken : class, new()
            => Configure(options => options.DefaultTokenType = typeof(TToken));
    }
}