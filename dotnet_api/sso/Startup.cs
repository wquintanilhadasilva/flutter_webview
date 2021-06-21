using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using sso.Models;
using sso.Services;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace sso
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var ssoProviders = new SsoProviders();
            Configuration.Bind("sso", ssoProviders);
            services.AddSingleton(ssoProviders);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var key = Encoding.ASCII.GetBytes(SecurityConfiguration.Secret);

            var builder = services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            })
            .AddCookie(SsoAuthenticationDefaults.AuthenticationScheme);

            foreach (SsoProvider provider in ssoProviders.Providers)
            {
                switch (provider.Name)
                {
                    case SsoProviderType.Google: // https://console.cloud.google.com/apis/credentials
                        builder.AddGoogle(options =>
                        {
                            options.SaveTokens = true;
                            options.ClientId = provider.ClientId;
                            options.ClientSecret = provider.ClientSecret;
                            options.Events.OnTicketReceived = (context) =>
                            {
                                Console.WriteLine(context.HttpContext.User);
                                return Task.CompletedTask;
                            };
                            options.Events.OnCreatingTicket = (context) =>
                            {
                                Console.WriteLine(context.Identity);
                                return Task.CompletedTask;
                            };
                        });
                        break;
                    case SsoProviderType.Facebook: //https://developers.facebook.com/apps/
                        builder.AddFacebook(options =>
                        {
                            options.SaveTokens = true;
                            options.ClientId = provider.ClientId;
                            options.ClientSecret = provider.ClientSecret;
                            options.Events.OnTicketReceived = (context) =>
                            {
                                Console.WriteLine(context.HttpContext.User);
                                return Task.CompletedTask;
                            };
                            options.Events.OnCreatingTicket = (context) =>
                            {
                                Console.WriteLine(context.Identity);
                                return Task.CompletedTask;
                            };
                        });
                        break;
                    case SsoProviderType.Twitter: // https://developer.twitter.com/en/portal/projects/
                        builder.AddTwitter(options =>
                        {
                            options.SaveTokens = true;
                            options.ConsumerKey = provider.ClientId;
                            options.ConsumerSecret = provider.ClientSecret;
                            options.Events.OnTicketReceived = (context) =>
                            {
                                Console.WriteLine(context.HttpContext.User);
                                return Task.CompletedTask;
                            };
                            options.Events.OnCreatingTicket = (context) =>
                            {
                                Console.WriteLine(context.Principal.Identity);
                                return Task.CompletedTask;
                            };
                        });
                        break;
                    case SsoProviderType.Legacy: // 

                        // Add OpenIdConnect
                        builder.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, "Acesso.gov", options =>
                        {

                            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                            options.Authority = provider.Authority;
                            options.RequireHttpsMetadata = provider.Https;
                            options.ClientId = provider.ClientId;
                            options.ClientSecret = provider.ClientSecret;
                            options.ResponseType = OpenIdConnectResponseType.Code;
                            options.GetClaimsFromUserInfoEndpoint = true;
                            options.Scope.Add("openid");
                            options.Scope.Add("profile");
                            options.SaveTokens = true;
                            options.TokenValidationParameters = new TokenValidationParameters
                            {
                                NameClaimType = "name",
                                RoleClaimType = "groups",
                                ValidateIssuer = true
                            };
                            options.Events.OnTicketReceived = (context) =>
                            {
                                Console.WriteLine(context.HttpContext.User);
                                return Task.CompletedTask;
                            };
                            options.Events.OnTicketReceived = (context) =>
                            {
                                Console.WriteLine(context.Principal.Identity);
                                return Task.CompletedTask;
                            };

                            // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress:
                            // http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name: 

                            //options.Events = new OpenIdConnectEvents()
                            //{
                            //    OnUserInformationReceived = context =>
                            //    {
                            //        var identity = (ClaimsIdentity)context.Principal.Identity;

                            //        foreach (var a in identity.Claims)
                            //        {
                            //            //code has been removed
                            //            Console.WriteLine(a);
                            //        }

                            //        return Task.CompletedTask;
                            //    },
                            //    OnTicketReceived = context =>
                            //    {
                            //        //code has been removed
                            //        return Task.FromResult(0);
                            //    }
                            //};

                        });

                        break;
                }
            }

            

            builder.AddCookie();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseMiddleware<JWTInHeaderMiddleware>(); //decode token

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
