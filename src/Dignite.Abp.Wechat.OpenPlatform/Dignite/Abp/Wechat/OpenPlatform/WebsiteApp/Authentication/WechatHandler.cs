using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Base64UrlTextEncoder = Microsoft.AspNetCore.Authentication.Base64UrlTextEncoder;

namespace Dignite.Abp.Wechat.OpenPlatform.WebsiteApp.Authentication;
public class WechatHandler : OAuthHandler<WechatOptions>
{
    /// <summary>
    /// Initializes a new instance of <see cref="WechatHandler"/>.
    /// </summary>
    /// <inheritdoc />
    public WechatHandler(
            IOptionsMonitor<WechatOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    private const string CorrelationMarker = "N";
    private const string CorrelationProperty = ".xsrf";

    /// <inheritdoc />
    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        // 网站应用微信登录开发指南:
        // https://developers.weixin.qq.com/doc/oplatform/Website_App/WeChat_Login/Wechat_Login.html

        var queryStrings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        queryStrings.Add("response_type", "code");
        queryStrings.Add("appid", Options.AppId);
        queryStrings.Add("redirect_uri", redirectUri);

        AddQueryString(queryStrings, properties, OAuthChallengeProperties.ScopeKey, FormatScope, Options.Scope);


        // 测试表明properties添加returnUrl和scheme后，state为1264字符，此时报错：state参数过长。
        // 所以properties只能存放在Cookie中，state作为Cookie值的索引键。
        // 腾讯规定state最长128字节，所以properties只能存放在Cookie中，state作为Cookie值的索引键。
        // https://mp.weixin.qq.com/wiki?t=resource/res_main&id=mp1421140842

        var state = Options.StateDataFormat.Protect(properties);
        var correlationId = properties.Items[CorrelationProperty];
        queryStrings.Add("state", correlationId);

        // Clean up old cookies with pattern: "Options.CorrelationCookie.Name + Scheme.Name + "." + correlationId + "." + CorrelationMarker"
        var deprecatedCookieNames = Context.Request.Cookies.Keys.Where(x => x.StartsWith(Options.CorrelationCookie.Name + Scheme.Name + "."));// && x.EndsWith("."+CorrelationMarker));
        deprecatedCookieNames.ToList().ForEach(x => Context.Response.Cookies.Delete(x));//, cookieOptions));
                                                                                        // Append a response cookie for state/properties
        var cookieOptions = Options.CorrelationCookie.Build(Context, Clock.UtcNow);
        Context.Response.Cookies.Append(FormatStateCookieName(correlationId), state, cookieOptions);

        var authorizationEndpoint = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, queryStrings!);
        return authorizationEndpoint;
    }

    private static void AddQueryString<T>(
        IDictionary<string, string> queryStrings,
        AuthenticationProperties properties,
        string name,
        Func<T, string> formatter,
        T defaultValue)
    {
        string value;
        var parameterValue = properties.GetParameter<T>(name);
        if (parameterValue != null)
        {
            value = formatter(parameterValue);
        }
        else if (!properties.Items.TryGetValue(name, out value))
        {
            value = formatter(defaultValue);
        }

        // Remove the parameter from AuthenticationProperties so it won't be serialized into the state
        properties.Items.Remove(name);

        if (value != null)
        {
            queryStrings[name] = value;
        }
    }
    #region To satisfy too big protected properties, we should store it to cookie '.{CorrelationCookieName}.{SchemeName}.{CorrelationMarker}.{CorrelationId|state}'
    protected virtual string FormatCorrelationCookieName(string correlationId)
    {
        return Options.CorrelationCookie.Name + Scheme.Name + "." + correlationId;
    }

    protected virtual string FormatStateCookieName(string correlationId)
    {
        //return Options.CorrelationCookie.Name + CorrelationMarker + "." + correlationId;
        return Options.CorrelationCookie.Name + Scheme.Name + "." + CorrelationMarker + "." + correlationId;
    }

    /// <inheritdoc/>
    protected override void GenerateCorrelationId(AuthenticationProperties properties)
    {
        if (properties == null)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);
        var correlationId = Base64UrlTextEncoder.Encode(bytes);

        var cookieOptions = Options.CorrelationCookie.Build(Context, Clock.UtcNow);

        properties.Items[CorrelationProperty] = correlationId;

        var cookieName = FormatCorrelationCookieName(correlationId);

        Response.Cookies.Append(cookieName, CorrelationMarker, cookieOptions);
    }

    /// <inheritdoc/>
    protected override bool ValidateCorrelationId(AuthenticationProperties properties)
    {
        if (properties == null)
        {
            throw new ArgumentNullException(nameof(properties));
        }

        if (!properties.Items.TryGetValue(CorrelationProperty, out var correlationId))
        {
            Logger.LogWarning($"The CorrectionId not found in '{Options.CorrelationCookie.Name!}'");
            return false;
        }

        properties.Items.Remove(CorrelationProperty);

        var cookieName = FormatCorrelationCookieName(correlationId);

        var correlationCookie = Request.Cookies[cookieName];
        if (string.IsNullOrEmpty(correlationCookie))
        {
            Logger.LogWarning($"The CorrelationCookie not found in '{cookieName}'");
            return false;
        }

        var cookieOptions = Options.CorrelationCookie.Build(Context, Clock.UtcNow);

        Response.Cookies.Delete(cookieName, cookieOptions);

        if (!string.Equals(correlationCookie, CorrelationMarker, StringComparison.Ordinal))
        {
            Logger.LogWarning($"Unexcepted CorrelationCookieValue: '{cookieName}'='{correlationCookie}'");
            return false;
        }

        return true;
    }
    #endregion

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var query = Request.Query;

        var correlationId = query["state"]; // ie. correlationId
        if (StringValues.IsNullOrEmpty(correlationId))
        {
            return HandleRequestResult.Fail("The oauth state was missing.");
        }

        var stateCookieName = FormatStateCookieName(correlationId);
        var state = Request.Cookies[stateCookieName];
        if (string.IsNullOrEmpty(state))
        {
            return HandleRequestResult.Fail($"The oauth state cookie was missing: Cookie: {stateCookieName}");
        }

        var properties = Options.StateDataFormat.Unprotect(state);

        if (properties == null)
        {
            return HandleRequestResult.Fail($"The oauth state cookie was invalid: Cookie: {stateCookieName}");
        }

        // OAuth2 10.12 CSRF
        if (!ValidateCorrelationId(properties))
        {
            return HandleRequestResult.Fail("Correlation failed.", properties);
        }

        // Cleanup state & correlation cookie
        var correlationCookieName = FormatCorrelationCookieName(correlationId);
        Response.Cookies.Delete(stateCookieName);
        Response.Cookies.Delete(correlationCookieName);


        var error = query["error"];
        if (!StringValues.IsNullOrEmpty(error))
        {
            // Note: access_denied errors are special protocol errors indicating the user didn't
            // approve the authorization demand requested by the remote authorization server.
            // Since it's a frequent scenario (that is not caused by incorrect configuration),
            // denied errors are handled differently using HandleAccessDeniedErrorAsync().
            // Visit https://tools.ietf.org/html/rfc6749#section-4.1.2.1 for more information.
            var errorDescription = query["error_description"];
            var errorUri = query["error_uri"];
            if (StringValues.Equals(error, "access_denied"))
            {
                var result = await HandleAccessDeniedErrorAsync(properties);
                if (!result.None)
                {
                    return result;
                }
                var deniedEx = new Exception("Access was denied by the resource owner or by the remote server.");
                deniedEx.Data["error"] = error.ToString();
                deniedEx.Data["error_description"] = errorDescription.ToString();
                deniedEx.Data["error_uri"] = errorUri.ToString();

                return HandleRequestResult.Fail(deniedEx, properties);
            }

            var failureMessage = new StringBuilder();
            failureMessage.Append(error);
            if (!StringValues.IsNullOrEmpty(errorDescription))
            {
                failureMessage.Append(";Description=").Append(errorDescription);
            }
            if (!StringValues.IsNullOrEmpty(errorUri))
            {
                failureMessage.Append(";Uri=").Append(errorUri);
            }

            var ex = new Exception(failureMessage.ToString());
            ex.Data["error"] = error.ToString();
            ex.Data["error_description"] = errorDescription.ToString();
            ex.Data["error_uri"] = errorUri.ToString();

            return HandleRequestResult.Fail(ex, properties);
        }


        var code = query["code"];

        if (StringValues.IsNullOrEmpty(code))
        {
            return HandleRequestResult.Fail("Code was not found.", properties);
        }

        var codeExchangeContext = new OAuthCodeExchangeContext(properties, code, BuildRedirectUri(Options.CallbackPath));
        using var tokens = await ExchangeCodeAsync(codeExchangeContext);

        if (tokens.Error != null)
        {
            return HandleRequestResult.Fail(tokens.Error, properties);
        }

        if (string.IsNullOrEmpty(tokens.AccessToken))
        {
            return HandleRequestResult.Fail("Failed to retrieve access token.", properties);
        }

        var identity = new ClaimsIdentity(ClaimsIssuer);

        if (Options.SaveTokens)
        {
            var authTokens = new List<AuthenticationToken>
            {
                new AuthenticationToken { Name = WeixinOpenTokenNames.access_token, Value = tokens.AccessToken },
                new AuthenticationToken { Name = WeixinOpenTokenNames.refresh_token, Value = tokens.RefreshToken },
                new AuthenticationToken { Name = WeixinOpenTokenNames.openid, Value = tokens.GetOpenId() },
                new AuthenticationToken { Name = WeixinOpenTokenNames.scope, Value = tokens.GetScope() }
            };
            
            if (!string.IsNullOrEmpty(tokens.GetUnionId()))
            {
                authTokens.Add(new AuthenticationToken { Name = WeixinOpenTokenNames.unionid, Value = tokens.GetUnionId() });
            }
            if (!string.IsNullOrEmpty(tokens.ExpiresIn))
            {
                int expiresIn;
                if (int.TryParse(tokens.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture, out expiresIn))
                {
                    // https://www.w3.org/TR/xmlschema-2/#dateTime
                    // https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx
                    var expiresAt = Clock.UtcNow + TimeSpan.FromSeconds(expiresIn);
                    authTokens.Add(new AuthenticationToken
                    {
                        Name = WeixinOpenTokenNames.expires_at,
                        Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                    });
                }
            }

            properties.StoreTokens(authTokens); //ExternalLoginInfo.AuthenticationTokens
        }


        var ticket = await CreateTicketAsync(identity, properties, tokens);
        if (ticket != null)
        {
            return HandleRequestResult.Success(ticket);
        }
        else
        {
            return HandleRequestResult.Fail("Failed to retrieve user information from remote server.", properties);
        }
    }

    /// <summary>
    /// Exchanges the authorization code for a authorization token from the remote provider.
    /// </summary>
    /// <param name="context">The <see cref="OAuthCodeExchangeContext"/>.</param>
    /// <returns>The response <see cref="OAuthTokenResponse"/>.</returns>
    protected virtual async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthCodeExchangeContext context)
    {
        var tokenRequestParameters = new Dictionary<string, string>()
            {
                { "appid", Options.AppId },
                { "secret", Options.AppSecret },
                { "code", context.Code },
                { "grant_type", "authorization_code" },
            };

        var requestUrl = QueryHelpers.AddQueryString(Options.TokenEndpoint, tokenRequestParameters);

        var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        requestMessage.Version = Backchannel.DefaultRequestVersion;
        var response = await Backchannel.SendAsync(requestMessage, Context.RequestAborted);
        var body = await response.Content.ReadAsStringAsync(Context.RequestAborted);

        return response.IsSuccessStatusCode switch
        {
            true => OAuthTokenResponse.Success(JsonDocument.Parse(body)),
            false => PrepareFailedOAuthTokenReponse(response, body)
        };
    }

    private static OAuthTokenResponse PrepareFailedOAuthTokenReponse(HttpResponseMessage response, string body)
    {
        var root = JsonDocument.Parse(body).RootElement;
        var error = root.GetString("error");

        if (error is null)
        {
            return null;
        }

        var result = new StringBuilder("OAuth token endpoint failure: ");
        result.Append(error);

        if (root.TryGetProperty("error_description", out var errorDescription))
        {
            result.Append(";Description=");
            result.Append(errorDescription);
        }

        if (root.TryGetProperty("error_uri", out var errorUri))
        {
            result.Append(";Uri=");
            result.Append(errorUri);
        }

        var exception = new Exception(result.ToString());
        exception.Data["error"] = error.ToString();
        exception.Data["error_description"] = errorDescription.ToString();
        exception.Data["error_uri"] = errorUri.ToString();

        if (exception is null)
        {
            var errorMessage = $"OAuth token endpoint failure: Status: {response.StatusCode};Headers: {response.Headers};Body: {body};";
            return OAuthTokenResponse.Failed(new Exception(errorMessage));
        }

        return OAuthTokenResponse.Failed(exception);
    }


    /// <inheritdoc />
    protected async override Task<AuthenticationTicket> CreateTicketAsync(
        ClaimsIdentity identity,
        AuthenticationProperties properties,
        OAuthTokenResponse tokens)
    {
        var requestParameters = new Dictionary<string, string>()
            {
                { "access_token", tokens.AccessToken },
                { "openid", tokens.GetOpenId() },
                { "lang", Options.LanguageCode.ToString() },
            };

        var requestUrl = QueryHelpers.AddQueryString(Options.UserInformationEndpoint, requestParameters);

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);

        var response = await Backchannel.SendAsync(request, Context.RequestAborted);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"An error occurred when retrieving Microsoft user information ({response.StatusCode}). Please check if the authentication information is correct and the corresponding Microsoft Account API is enabled.");
        }

        using (var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync(Context.RequestAborted)))
        {
            var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, payload.RootElement);
            context.RunClaimActions();
            await Events.CreatingTicket(context);
            return new AuthenticationTicket(context.Principal!, context.Properties, Scheme.Name);
        }
    }
}
