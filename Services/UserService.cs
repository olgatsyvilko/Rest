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

        public RestResponse GetFilteredUsers(string filterOption, object filterValue)
        {
            return CreateAndExecuteRequest($"{ResourceNames.UserService}" + $"?{filterOption}={filterValue}", Method.Get, Scope.Read);
        }

        public RestResponse GetFilteredUsersByTwoFilters(string firstFilter, object firstFilterValue, string secondFilter, object secondFilterValue)
        {
            return CreateAndExecuteRequest($"{ResourceNames.UserService}" 
                + $"?{firstFilter}={firstFilterValue}" 
                + $"&{secondFilter}={secondFilterValue}", Method.Get, Scope.Read);
        }

        public RestResponse CreateUser(object user)
        {
            return CreateAndExecuteRequest(ResourceNames.UserService, Method.Post, Scope.Write, user);
        }
    }
}
