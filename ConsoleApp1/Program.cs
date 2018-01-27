using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenValidation
{
    class Program
    {
        static void Main(string[] args)
        {
            const string auth0Domain = "https://sts.windows.net/1a118090-7001-4b0d-8fdb-22f2598d838b/"; // Your Auth0 domain
            const string auth0Audience = "02d48a4b-6dc2-49ad-af0d-a7cfc50e7017"; // Your API Identifier
            const string testToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6Ino0NHdNZEh1OHdLc3VtcmJmYUs5OHF4czVZSSIsImtpZCI6Ino0NHdNZEh1OHdLc3VtcmJmYUs5OHF4czVZSSJ9.eyJhdWQiOiIwMmQ0OGE0Yi02ZGMyLTQ5YWQtYWYwZC1hN2NmYzUwZTcwMTciLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8xYTExODA5MC03MDAxLTRiMGQtOGZkYi0yMmYyNTk4ZDgzOGIvIiwiaWF0IjoxNTE1NzM1NjI4LCJuYmYiOjE1MTU3MzU2MjgsImV4cCI6MTUxNTczOTUyOCwiYWlvIjoiQVRRQXkvOEdBQUFBVlZPWWNyNHNobEZ4SmpnWE9uSS96ZEN0NTZxaFRhOTRCK0MwRkprYzZTY25xRzhhSkZtUlpzVEhtY29odkxleCIsImFtciI6WyJwd2QiXSwiZW1haWwiOiJTTWFoYXJ6YW5AYmx1ZXBhbmRhdWMuY29tIiwiZmFtaWx5X25hbWUiOiJNYWhhcnphbiIsImdpdmVuX25hbWUiOiJTTWFoYXJ6YW5AYmx1ZXBhbmRhdWMuY29tIiwiaWRwIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvNDMxMTE4MTQtOTZjMS00YTQ3LWEyMDctODc3ZDQ0MzIxMjgyLyIsImlwYWRkciI6IjIwMi4xNjYuMjAxLjEwOCIsIm5hbWUiOiJTTWFoYXJ6YW5AYmx1ZXBhbmRhdWMuY29tIE1haGFyemFuIiwibm9uY2UiOiIzOGJlMWI2Ny01NDc1LTQzNzEtYjgwMy0xMTEyNGE4M2MyYTkiLCJvaWQiOiJmNmUxYWU5NC1kNTI1LTRiYTAtYWI4Ny00YzgzZDNjNjQ0NTAiLCJzdWIiOiJMRHhkY1pZa3ZubWFVSHNUZHZ2QzVOcDMzWTNxODRyNnJ2VjFZMVZMc3RzIiwidGlkIjoiMWExMTgwOTAtNzAwMS00YjBkLThmZGItMjJmMjU5OGQ4MzhiIiwidW5pcXVlX25hbWUiOiJTTWFoYXJ6YW5AYmx1ZXBhbmRhdWMuY29tIiwidXRpIjoiMHdERTBqZ1o0RVNQN3NEQzNlOENBQSIsInZlciI6IjEuMCJ9.CI6QAFU0llIKoCzLv5t5Aa8gMEw5R02R0p6MuXcKcXGf_TaFYtJs2B9qRQh3fpX-mKv7gSbMYIIhxqFSOJIuAMSiTJyliURaNyL_fHXBGdvbmAYlgzdPsKrGhu_pF13YspJ7-JBEhqdvsD_hB_583eynOPErKOHI0miQdnE102Z9bYzvuaFKPQafC2ikVWbhS7xfAi5S3ZvTG9s1EdEP68dLbMtG8usO_uAGGVIWz0MLYIIsjJfwq4JZvjFK-JX0bKqgaPahN1SKPw73XcSQqtuFkPxMOTsZQifnXxxKyMmpCCCpoHBKArrIGtEk-H4K-DJYKs1h5JgPoNijUTGiew"; // Obtain a JWT to validate and put it in here


            try
            {
                // Download the OIDC configuration which contains the JWKS
                // NB!!: Downloading this takes time, so do not do it very time you need to validate a token, Try and do it only once in the lifetime
                //     of your application!!
                IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>($"{auth0Domain}.well-known/openid-configuration?p=B2C_1_signin", new OpenIdConnectConfigurationRetriever());
                OpenIdConnectConfiguration openIdConfig = AsyncHelper.RunSync(async () => await configurationManager.GetConfigurationAsync(CancellationToken.None));

                // Configure the TokenValidationParameters. Assign the SigningKeys which were downloaded from Auth0. 
                // Also set the Issuer and Audience(s) to validate
                TokenValidationParameters validationParameters =
                    new TokenValidationParameters
                    {
                        ValidIssuer = auth0Domain,
                        ValidAudiences = new[] { auth0Audience },
                        IssuerSigningKeys = openIdConfig.SigningKeys
                    };

                // Now validate the token. If the token is not valid for any reason, an exception will be thrown by the method
                SecurityToken validatedToken;
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var user = handler.ValidateToken(testToken, validationParameters, out validatedToken);

                // The ValidateToken method above will return a ClaimsPrincipal. Get the user ID from the NameIdentifier claim
                // (The sub claim from the JWT will be translated to the NameIdentifier claim)
                Console.WriteLine($"Token is validated. User Id {user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error occurred while validating token: {e.Message}");
                throw;
            }

            Console.WriteLine();
            Console.WriteLine("Press ENTER to continue...");
            Console.ReadLine();
        }
    }
}
