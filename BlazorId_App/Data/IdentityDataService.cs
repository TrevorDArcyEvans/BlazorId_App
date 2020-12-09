using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorId_App.Data
{
    public class IdentityDataService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _userClaims;

        public IdentityDataService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient= httpClient?? throw new System.ArgumentNullException(nameof(httpClient));
            _httpContextAccessor = httpContextAccessor ?? throw new System.ArgumentNullException(nameof(httpContextAccessor));
        }

        // Get user claims json from the API get method
        public async Task<string>GetAPIUserClaimsJson()
        {
            var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            if (accessToken != null)
            {
	            _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            }

            HttpResponseMessage response = null ;
            try
            {
                response = await _httpClient.GetAsync($"identity");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException e)
            {
                return e.Message;
            }
            var rawJson = await response.Content.ReadAsStringAsync();
            var parsedJson = JToken.Parse(rawJson);
            return  parsedJson.ToString(Formatting.Indented);
            
        }

        /// <summary>
        /// Gets user claims from the current httpcontext
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetAPPUserClaimsJson()
        {
            var claimsList = _httpContextAccessor.HttpContext.User.Claims;

            var rawList = (from c in claimsList select new { c.Type, c.Value });
            var json = JsonConvert.SerializeObject(rawList, Formatting.Indented);

            return json;
        }
    }
}
