using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Security.Claims;
using System.Security.Principal;
using thongbao.be.application.Auth.Interfaces;
using thongbao.be.domain.Auth;
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


        public AuthorizationController(IOpenIddictApplicationManager applicationManager, IUsersService usersService, IOptions<AuthServerSettings> options)
        {
            _applicationManager = applicationManager;
            _usersService = usersService;
            _authServerSettings = options.Value;
        }
            

        [HttpPost("~/connect/token"), Produces("application/json")]
        public async Task<IActionResult> Exchange([FromServices] UserManager<AppUser> userManager)
        {
            var request = HttpContext.GetOpenIddictServerRequest();
            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            var identity = new ClaimsIdentity(TokenValidationParameters.DefaultAuthenticationType, Claims.Name, Claims.Role);

            try
            {
                if (request.IsClientCredentialsGrantType())
                {
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

                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsAuthorizationCodeGrantType())
                {
                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                    string subject = result.Principal!.GetClaim(Claims.Subject)!;

                    // Use the client_id as the subject identifier.
                    identity.SetClaim(Claims.Subject, subject);
                    //identity.SetClaim(Claims.Name, user.FullName);
                    //identity.SetClaim(Claims.Username, user.UserName);
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

                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsPasswordGrantType())
                {
                    // Note: the client credentials are automatically validated by OpenIddict:
                    // if client_id or client_secret are invalid, this action won't be invoked.

                    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                        throw new InvalidOperationException("The application cannot be found.");

                    string username = request.Username!;
                    string password = request.Password!;

                    var user = await userManager.FindByNameAsync(username) ??
                        throw new UserFriendlyException(ErrorCodes.NotFound, "Tài khoản không tồn tại");


                    bool isValid = await userManager.CheckPasswordAsync(user, password);
                    if (!isValid)
                    {
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

                    return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
                }
                else if (request.IsRefreshTokenGrantType())
                {
                    var result = await HttpContext.AuthenticateAsync(
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );

                    string userid = result.Principal!.GetClaim(Claims.Subject)!;
                    string username = result.Principal!.GetClaim(Claims.Username)!;

                    var application = await _applicationManager.FindByClientIdAsync(request.ClientId) ??
                        throw new InvalidOperationException("The application cannot be found.");

                    var user = await userManager.FindByIdAsync(userid)
                        ?? throw new UserFriendlyException(ErrorCodes.NotFound, "Tài khoản không tồn tại");

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

                    return SignIn(
                        new ClaimsPrincipal(identity),
                        OpenIddictServerAspNetCoreDefaults.AuthenticationScheme
                    );
                }
            }
            catch (UserFriendlyException ex)
            {
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
            var request = HttpContext.GetOpenIddictServerRequest()
                  ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // If user not logged in, redirect them to Microsoft
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                var props = new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalCallback", new { returnUrl = Request.Path + QueryString.Create(Request.Query.ToDictionary(k => k.Key, v => v.Value.ToString())) })
                };
                return Challenge(props, MicrosoftAccountDefaults.AuthenticationScheme);
            }
            var ss = User.Claims.ToList();
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

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });

            var principal = new ClaimsPrincipal(identity);

            // ✅ Tell OpenIddict to issue tokens
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }

        [HttpGet("~/external-callback")]
        public async Task<IActionResult> ExternalCallback([FromServices] UserManager<AppUser> userManager, string? returnUrl = "/", string? remoteError = null)
        {

            // Authenticate using Google scheme
            var result = await HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return BadRequest("MS authentication failed");

            
            var claims = result.Principal!.Identities.First().Claims;
            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            //var user = await _usersService.FindByMsAccount(email!);
            var user = userManager.Users.AsNoTracking().FirstOrDefault(x => x.MsAccount == email);

            if (user == null)
            {
                user = await _usersService.Create(new application.Auth.Dtos.User.CreateUserDto
                {
                    UserName = email,
                    Email = email,
                    MsAccount = email!,
                    FullName = name ?? "",
                    Password = "Password@7"
                });
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Use the client_id as the subject identifier.
            identity.SetClaim(Claims.Subject, user.Id);
            identity.SetClaim(Claims.Name, user.FullName);
            identity.SetClaim(Claims.Username, user.UserName);

            identity.SetDestinations(static claim => claim.Type switch
            {
                // Allow the "name" claim to be stored in both the access and identity tokens
                // when the "profile" scope was granted (by calling principal.SetScopes(...)).
                Claims.Name when claim.Subject.HasScope(Scopes.Profile)
                    => [Destinations.AccessToken, Destinations.IdentityToken],

                // Otherwise, only store the claim in the access tokens.
                _ => [Destinations.AccessToken]
            });

            // Sign in the user temporarily with cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // Go back to original /connect/authorize request
            return Redirect(returnUrl);

            //return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
    }
}
