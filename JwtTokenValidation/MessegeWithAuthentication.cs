using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace JwtTokenValidation
{
    public static class MessegeWithAuthentication
    {
        [FunctionName("getMessageWithValidation")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            // Authentication 
            ClaimsPrincipal principal;
            List<UserClaims> newListOfUserClaims = new List<UserClaims>();

            if ((principal = await Security.ValidateTokenAsync(req.Headers.Authorization)) == null)
            {
                return req.CreateResponse(HttpStatusCode.Unauthorized);
            }
            if (principal.Identity.IsAuthenticated)
            {
                var userClaimFromClaimsPrincipals = (from claim in principal.Claims select claim).ToArray();
                //var claims = principal.Claims;
                foreach(var extractClaim in userClaimFromClaimsPrincipals)
                {
                    log.Info(extractClaim.Type + ":" + extractClaim.Value);
                    UserClaims newClaim = new UserClaims();
                    newClaim.Type = extractClaim.Type;
                    newClaim.Value = extractClaim.Value;
                    newListOfUserClaims.Add(newClaim);
                }
            }
            return req.CreateResponse(HttpStatusCode.OK, newListOfUserClaims);
        }
        public static void ClaimAdd()
        {
            Claim displayName = ClaimsPrincipal.Current.FindFirst(ClaimsPrincipal.Current.Identities.First().NameClaimType);
            //ViewBag.DisplayName = displayName != null ? displayName.Value : string.Empty;
            //ClaimType, Value, ValueType, Issuer
            Claim localClaim = new Claim(ClaimTypes.Webpage, "http://localhost", ClaimValueTypes.String, "AADGuide");

            //Method 1 - short version
            ClaimsPrincipal.Current.Identities.First().AddClaim(localClaim);
        }
        public class UserClaims
        {
            public string Type { get; set; }

            public string Value { get; set; }
        }
    }
}
