using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Server.AspNetCore;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Security.Principal;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.domain.Auth;
using thongbao.be.shared.Constants.Auth;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using thongbao.be.shared.Settings;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace thongbao.be.Controllers.Auth
{
    [Route("")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;
        private readonly IUsersService _usersService;
        private readonly AuthServerSettings _authServerSettings;
        private readonly ILogger<AuthorizationController> _logger;

        public AuthorizationController(
            IOpenIddictApplicationManager applicationManager,
            IUsersService usersService,
            IOptions<AuthServerSettings> options,
            ILogger<AuthorizationController> logger)
        {
            _applicationManager = applicationManager;
            _usersService = usersService;
            _authServerSettings = options.Value;
            _logger = logger;
        }

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange([FromServices] UserManager<AppUser> userManager)
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            _logger.LogInformation("Token exchange request received. Grant type: {GrantType}", request.GrantType);

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            try
            {
                if (request.IsClientCredentialsGrantType())
                {
                    _logger.LogInformation("Processing client credentials grant for client: {ClientId}", request.ClientId);

                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                        throw new InvalidOperationException("The application cannot be found.");

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application));
                    identity.SetClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application));

                    identity.SetDestinations(static claim => claim.Type switch
                    {
                        // Allow the "name" claim to be stored in both the access and identity tokens
                        // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                        Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                            => [Destinations.AccessToken, Destinations.IdentityToken],

                        // Otherwise, only store the claim in the access tokens.
                        _ => [Destinations.AccessToken]
                    });

                    _logger.LogInformation("Client credentials grant successful for client: {ClientId}", request.ClientId);
                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsAuthorizationCodeGrantType())
                {
                    _logger.LogInformation("Processing authorization code grant");

                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                    string subject = result.Principal!.GetClaim(Claims.Subject)!;

                    var user = await userManager.FindByIdAsync(subject)
                        ?? throw new UserFriendlyException(ErrorCodes.AuthErrorUserNotFound);

                    _logger.LogInformation("Authorization code grant for user: {UserId}", user.Id);

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(Claims.Subject, subject);
                    identity.SetClaim(Claims.Subject, subject);
                    identity.SetClaim(Claims.Name, user.FullName);
                    identity.SetClaim(Claims.Username, user.UserName);
                    identity.SetClaim(CustomClaimTypes.UserType, "SV");
                    identity.SetScopes(
                            new[]
                            {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                            }.Intersect(request.GetScopes())
                        );
                    identity.SetDestinations(static claim => claim.Type switch
                    {
                        // Allow the "name" claim to be stored in both the access and identity tokens
                        // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                        Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                            => [Destinations.AccessToken, Destinations.IdentityToken],

                        // Otherwise, only store the claim in the access tokens.
                        _ => [Destinations.AccessToken]
                    });

                    _logger.LogInformation("Authorization code grant successful for user: {UserId}", user.Id);
                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsPasswordGrantType())
                {
                    string username = request.Username!;
                    _logger.LogInformation("Processing password grant for user: {Username}", username);

                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                        throw new InvalidOperationException("The application cannot be found.");

                    string password = request.Password!;

                    var user = await userManager.FindByNameAsync(username) ??
                        throw new UserFriendlyException(ErrorCodes.NotFound, "Tài khoản không tồn tại");

                    bool isValid = await userManager.CheckPasswordAsync(user, password);
                    if (!isValid)
                    {
                        _logger.LogWarning("Invalid password attempt for user: {Username}", username);
                        throw new UserFriendlyException(ErrorCodes.AuthInvalidPassword, "Mật khẩu không chính xác");
                    }

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(Claims.Subject, user.Id);
                    identity.SetClaim(Claims.Name, user.FullName);
                    identity.SetClaim(Claims.Username, user.UserName);
                    identity.SetScopes(
                            new[]
                            {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                            }.Intersect(request.GetScopes())
                        );
                    identity.SetDestinations(static claim => claim.Type switch
                    {
                        // Allow the "name" claim to be stored in both the access and identity tokens
                        // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                        Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                            => [Destinations.AccessToken, Destinations.IdentityToken],

                        // Otherwise, only store the claim in the access tokens.
                        _ => [Destinations.AccessToken]
                    });

                    _logger.LogInformation("Password grant successful for user: {Username}", username);
                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsRefreshTokenGrantType())
                {
                    _logger.LogInformation("Processing refresh token grant");

                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );

                    string userid = result.Principal!.GetClaim(Claims.Subject)!;
                    string username = result.Principal!.GetClaim(Claims.Username)!;

                    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                        throw new InvalidOperationException("The application cannot be found.");

                    var user = await userManager.FindByIdAsync(userid)
                        ?? throw new UserFriendlyException(ErrorCodes.NotFound, "Tài khoản không tồn tại");

                    _logger.LogInformation("Refresh token grant for user: {UserId}", user.Id);

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(Claims.Subject, user.Id);
                    identity.SetClaim(Claims.Name, user.FullName);
                    identity.SetClaim(Claims.Username, user.UserName);

                    identity.SetScopes(
                            new[]
                            {
                            Scopes.OpenId,
                            Scopes.Email,
                            Scopes.Profile,
                            Scopes.Roles,
                            Scopes.OfflineAccess
                            }.Intersect(request.GetScopes())
                        );
                    identity.SetDestinations(static claim => claim.Type switch
                    {
                        // Allow the "name" claim to be stored in both the access and identity tokens
                        // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                        Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                            => [Destinations.AccessToken, Destinations.IdentityToken],

                        // Otherwise, only store the claim in the access tokens.
                        _ => [Destinations.AccessToken]
                    });

                    _logger.LogInformation("Refresh token grant successful for user: {UserId}", user.Id);
                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
            }
            catch (UserFriendlyException ex)
            {
                _logger.LogWarning("User friendly exception in token exchange: {Message}", ex.MessageLocalize);

                var properties = new AuthenticationProperties(
                    new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                            Errors.InvalidGrant,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                            ex.MessageLocalize
                    }
                );
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in token exchange");

                var properties = new AuthenticationProperties(
                   new Dictionary<string, string?>
                   {
                       [OpenIddictServerAspNetCoreConstants.Properties.Error] =
                           Errors.InvalidGrant,
                       [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                           ex.Message
                   }
               );
                return Forbid(properties, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            _logger.LogWarning("Unsupported grant type: {GrantType}", request.GrantType);
            return BadRequest(
                   new OpenIddictResponse
                   {
                       Error = Errors.UnsupportedGrantType,
                       ErrorDescription = "The specified grant type is not supported."
                   }
               );
        }

        [HttpGet("~/connect/authorize")]
        public async Task<IActionResult> ConnectAuthorize([FromServices] UserManager<AppUser> userManager, string returnUrl = "/")
        {
            _logger.LogInformation("Authorization request received. Return URL: {ReturnUrl}", returnUrl);

            var request = HttpContext.GetOpenIddictServerRequest()
                  ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // If user not logged in, redirect them to Microsoft
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                _logger.LogInformation("User not authenticated, redirecting to Microsoft login");

                var props = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalCallback", new { returnUrl = Request.Path + QueryString.Create(Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString())) })
                };
                return Challenge(props, MicrosoftAccountDefaults.AuthenticationScheme);
            }

            _logger.LogInformation("User authenticated, processing authorization for user: {UserId}", User.FindFirst(Claims.Subject)?.Value);

            // At this point, the user info is already in cookie (from ExternalCallback)
            var identity = new ClaimsIdentity(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Copy claims from cookie identity into OpenIddict identity
            foreach (var claim in User.Claims)
            {
                identity.SetClaim(claim.Type, claim.Value);
            }

            // Use the client_id as the subject identifier.
            //identity.SetClaim(Claims.Subject, "test");
            //identity.SetClaim(Claims.Name, "nghia test");

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],
                CustomClaimTypes.UserType when true
                => [Destinations.AccessToken, Destinations.IdentityToken],
                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });

            var principal = new ClaimsPrincipal(identity);

            _logger.LogInformation("Authorization successful, issuing tokens for user: {UserId}", User.FindFirst(Claims.Subject)?.Value);

            // ✅ Tell OpenIddict to issue tokens
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpGet("~/external-callback")]
        public async Task<IActionResult> ExternalCallback([FromServices] UserManager<AppUser> userManager, string? returnUrl = "/", string? remoteError = null)
        {
            _logger.LogInformation("External callback received. Return URL: {ReturnUrl}", returnUrl);

            if (!string.IsNullOrEmpty(remoteError))
            {
                _logger.LogWarning("Remote authentication error: {RemoteError}", remoteError);
                return BadRequest($"Remote authentication error: {remoteError}");
            }

            // Authenticate using Microsoft scheme
            var result = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            if (!result.Succeeded)
            {
                _logger.LogError("Microsoft authentication failed");
                return BadRequest("MS authentication failed");
            }

            var claims = result.Principal!.Identities.First().Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            _logger.LogInformation("External authentication successful for email: {Email}", email);

            //var user = await _usersService.FindByMsAccount(email!);
            var user = userManager.Users.AsNoTracking().FirstOrDefault(x => x.MsAccount == email);

            if (user == null)
            {
                _logger.LogInformation("Creating new user for email: {Email}", email);

                user = await _usersService.Create(new application.Auth.Dtos.User.CreateUserDto
                {
                    UserName = email,
                    Email = email,
                    MsAccount = email!,
                    FullName = name ?? "",
                    Password = "Password@7"
                });

                _logger.LogInformation("New user created with ID: {UserId}", user.Id);
            }
            else
            {
                _logger.LogInformation("Existing user found with ID: {UserId}", user.Id);
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, user.Id);
            identity.SetClaim(Claims.Name, user.FullName);
            identity.SetClaim(Claims.Username, user.UserName);
            identity.SetClaim(CustomClaimTypes.UserType, "SV");

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                CustomClaimTypes.UserType when true
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });

            // Sign in the user temporarily with cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            _logger.LogInformation("User signed in with cookie authentication, redirecting to: {ReturnUrl}", returnUrl);

            // Go back to original /connect/authorize request
            return Redirect(returnUrl);

            //return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}