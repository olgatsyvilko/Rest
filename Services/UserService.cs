using Rest.Core;
using Rest.Enums;
using RestSharp;

namespace Rest.Services
{
    public class UserService : BaseService
    {
        public RestResponse GetUsers()
        {
            return CreateAndExecuteRequest(ResourceNames.UserService, Method.Get, Scope.Read);
        }

        public RestResponse CreateUser(object user)
        {
            return CreateAndExecuteRequest(ResourceNames.UserService, Method.Post, Scope.Write, user);
        }
    }
}
