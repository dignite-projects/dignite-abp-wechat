using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
public static class WechatExtensions
{

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WechatDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder)
        => builder.AddWechat(WechatDefaults.AuthenticationScheme, _ => { });

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WechatDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WechatOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, Action<WechatOptions> configureOptions)
        => builder.AddWechat(WechatDefaults.AuthenticationScheme, configureOptions);

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WechatDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WechatOptions"/>.</param>
    /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, string authenticationScheme, Action<WechatOptions> configureOptions)
        => builder.AddWechat(authenticationScheme, WechatDefaults.DisplayName, configureOptions);

    /// <summary>
    /// Adds Wechat OAuth-based authentication to <see cref="AuthenticationBuilder"/> using the default scheme.
    /// The default scheme is specified by <see cref="WechatDefaults.AuthenticationScheme"/>.
    /// <para>
    /// Wechat authentication allows application users to sign in with their Wechat account.
    /// </para>
    /// </summary>
    /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
    /// <param name="authenticationScheme">The authentication scheme.</param>
    /// <param name="displayName">A display name for the authentication handler.</param>
    /// <param name="configureOptions">A delegate to configure <see cref="WechatOptions"/>.</param>
    public static AuthenticationBuilder AddWechat(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<WechatOptions> configureOptions)
        => builder.AddOAuth<WechatOptions, WechatHandler>(authenticationScheme, displayName, configureOptions);
}
