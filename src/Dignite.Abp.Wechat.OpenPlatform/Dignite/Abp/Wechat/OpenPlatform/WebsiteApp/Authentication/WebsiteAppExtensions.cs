using System;
using Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;
public static class WebsiteAppExtensions
{

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WebsiteAppDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder)
        => builder.AddWechat(WebsiteAppDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WebsiteAppDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WebsiteAppOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, Action<WebsiteAppOptions> configureOptions)
        => builder.AddWechat(WebsiteAppDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WebsiteAppDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WebsiteAppOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, string authenticationScheme, Action<WebsiteAppOptions> configureOptions)
        => builder.AddWechat(authenticationScheme, WebsiteAppDefaults.DisplayName, configureOptions);

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WebsiteAppDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WebsiteAppOptions"/>.</param>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WebsiteAppOptions> configureOptions)
        => builder.AddOAuth<WebsiteAppOptions, WebsiteAppHandler>(authenticationScheme, displayName, configureOptions);
}
