using Rest.Enums;
using RestSharp;

namespace Rest.Services
{
    public class ZipCodeService : BaseService
    {
        public RestResponse GetZipCodes()
        {
            var resource = $"/zip-codes";
            return CreateAndExecuteRequest(resource, Method.Get, Scope.Read);
        }

        public RestResponse AddZipCodes(object codes)
        {
            var resource = $"/zip-codes/expand";
            return CreateAndExecuteRequest(resource, Method.Post, Scope.Write, codes);
        }
    }
}
