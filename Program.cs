using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSingleton<IPostConfigureOptions<OpenIdConnectOptions>, Auth0PostConfiguration>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(opts => {
            opts.DefaultAuthenticateScheme = opts.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opts.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        })
       .AddCookie()
       .AddOpenIdConnect(opts => {
            var c = builder.Configuration;
            opts.Authority = c["Auth0:Authority"];
            opts.ClientId = c["Auth0:ClientId"];
            opts.ClientSecret = c["Auth0:ClientSecret"];
            opts.ResponseType = "code";
            opts.SignedOutRedirectUri = $"{opts.Authority}v2/logout?client_id={opts.ClientId}";

            opts.SaveTokens = true;
            // opts.GetClaimsFromUserInfoEndpoint = true;
        });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

sealed class Auth0PostConfiguration : IPostConfigureOptions<OpenIdConnectOptions>
{
    readonly IPostConfigureOptions<OpenIdConnectOptions> baseConfigurer;
    public Auth0PostConfiguration(IDataProtectionProvider dp) {
        baseConfigurer = new OpenIdConnectPostConfigureOptions(dp);
    }

    public void PostConfigure(string? name, OpenIdConnectOptions options) {
        baseConfigurer.PostConfigure(name, options);
        options.ConfigurationManager = new Auth0ConfigurationManager(options, options.ConfigurationManager!);
    }
}

sealed class Auth0ConfigurationManager : IConfigurationManager<OpenIdConnectConfiguration>
{
    readonly OpenIdConnectOptions options;
    readonly IConfigurationManager<OpenIdConnectConfiguration> baseConfigurator;
    public Auth0ConfigurationManager(OpenIdConnectOptions options, IConfigurationManager<OpenIdConnectConfiguration> baseConfigurator) {
        this.options = options;
        this.baseConfigurator = baseConfigurator;
    }

    public async Task<OpenIdConnectConfiguration> GetConfigurationAsync(CancellationToken cancel) {
        var config = await baseConfigurator.GetConfigurationAsync(cancel);
        // Auth0 OIDC configuration does not return End Session endpoint!
        if (string.IsNullOrEmpty(config.EndSessionEndpoint))
            config.EndSessionEndpoint = $"{options.Authority}v2/logout?client_id={options.ClientId}";
        return config;
    }

    public void RequestRefresh() => baseConfigurator.RequestRefresh();
}