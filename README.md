# Auth0 with Blazor Server using OpenIdConnect library

This approach mirros that of [Blazor Server with Auth0](https://github.com/ruxo/blazor-server-auth0)
but it does not utilize `Auth0.AspNetCore.Authentication` library. By adopting this 
strategy, the solution gains increased flexibility as it can switch to any OIDC provider, and
is not necessarily tied with Auth0 service.

## Configuration Steps
1. Incorporate the Nuget package `Microsoft.AspNetCore.Authentication.OpenIdConnect`.
2. To configure ASP.NET Authentication, invoke the `AddAuthentication`, `AddCookie`,
   and `AddOpenIdConnect` methods, and pass the necessary options. For a concrete 
   example, refer to `Program.cs`.
   * It's important to note that there are two additional classes, `Auth0PostConfiguration` and
     `Auth0ConfigurationManager`, which are specifically implemented for the Auth0 service.
     Since this service requires the `EndSessionEndpoint` for proper logout, and as 
     it doesn't expose the endpoint through its standard OIDC configuration, these 
     additional classes serve as a workaround to correctly inject the endpoint into
     the options.

For the remaining steps, you can refer to the [Blazor Server with Auth0](https://github.com/ruxo/blazor-server-auth0) example.

# Additional Resources
* [Blazor Server with Auth0](https://github.com/ruxo/blazor-server-auth0)
* [Auth0 (or any OIDC) with Blazor WASM standalone](https://github.com/ruxo/blazor-wasm-auth0)
* [Auth0 with Blazor Hosted solution - Abridge version](https://github.com/ruxo/blazor-hosted-auth0)