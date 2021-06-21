using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sso.Models;
using sso.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace sso.Controllers
{
    public class AuthController : Controller
    {

        private readonly IAuthenticationSchemeProvider authenticationSchemeProvider;

        public AuthController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            this.authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public async Task<IActionResult> Login()
        {
            var allSchemeProvider = (await authenticationSchemeProvider.GetAllSchemesAsync()).Select(n => n.DisplayName).Where(n => !String.IsNullOrEmpty(n));
            return View(allSchemeProvider);
        }

        public IActionResult SignIn(string provider)
        {
            string returnUrl = "/login-success";
            provider = "Acesso.gov".Equals(provider) ? OpenIdConnectDefaults.AuthenticationScheme : provider;
            return Challenge(new AuthenticationProperties {
                    AllowRefresh = true,
                    ExpiresUtc = DateTime.Now.ToLocalTime().AddHours(2),
                    IsPersistent = true,
                    RedirectUri = Url.Action(nameof(LoginCallback), new { provider, returnUrl })
            }, provider);
        }

        public async Task<IActionResult> SignOutLocal()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Response.Cookies.Delete("jwt-token");
            base.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LoginCallback(string provider, string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(provider);

            if (!authenticateResult.Succeeded)
                return BadRequest();

            var id = authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier);
            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email);
            var name = authenticateResult.Principal.Identity.Name;

            var obj = new UserProfile
            {
                EmailAddress = email != null ? email.Value : "",
                Name = name ?? "",
                OIdProvider = provider,
                OId = id != null ? id.Value : "",
                Role = "employee" // "employee,manager,auth"
            };

            string token = TokenService.GenerateToken(obj);

            HttpContext.Response.Cookies.Append("jwt-token", token, new CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                // SameSite = SameSiteMode.Lax
            });

            // await _repo.GetOrCreateExternalUserAsync(obj, HttpContext);

            return LocalRedirect(returnUrl);
        }
    }
}
