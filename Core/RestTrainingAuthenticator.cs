using Rest.Enums;
using Rest.Models;
using RestSharp;
using RestSharp.Authenticators;

namespace Rest.Core
{
    public class RestTrainingAuthenticator(Scope scope) : AuthenticatorBase(string.Empty)
    {
        private readonly string _baseUrl = Configuration.BaseUrl;
        private readonly string _username = Configuration.Username;
        private readonly string _password = Configuration.Password;
        private readonly Scope scope = scope;

        protected override async ValueTask<Parameter> GetAuthenticationParameter(string accessToken)
        {
            Token = string.IsNullOrEmpty(Token) ? await GetToken() : Token;
            return new HeaderParameter(KnownHeaders.Authorization, Token);
        }

        private async Task<string> GetToken()
        {
            var options = new RestClientOptions(_baseUrl)
            {
                Authenticator = new HttpBasicAuthenticator(_username, _password),
            };
            using var client = new RestClient(options);

            var request = new RestRequest("/oauth/token").AddParameter("grant_type", "client_credentials");
            request.AddParameter("scope", scope.ToString().ToLower());
            var response = await client.PostAsync<TokenResponse>(request);
            
            return $"{response!.TokenType} {response!.AccessToken}";
        }
    }
}