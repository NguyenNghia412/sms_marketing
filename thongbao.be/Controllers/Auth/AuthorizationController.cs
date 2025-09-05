using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using System.Security.Principal;
using thongbao.be.domain.Auth;
using thongbao.be.shared.HttpRequest.Error;
using thongbao.be.shared.HttpRequest.Exception;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace thongbao.be.Controllers.Auth
{
    [Route("")]
    [ApiController]
    public class AuthorizationController : Controller
    {
        private readonly IOpenIddictApplicationManager _applicationManager;

        public AuthorizationController(IOpenIddictApplicationManager applicationManager)
            => _applicationManager = applicationManager;

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
        [HttpPost("/test")]
        public IActionResult TestEndpoint()
        {
            return Ok(new { message = "Test successful", time = DateTime.Now });
        }
    }
}
