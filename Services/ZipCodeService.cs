using Rest.Core;
using Rest.Enums;
using RestSharp;

namespace Rest.Services
{
    public class ZipCodeService : BaseService
    {
        public RestResponse GetZipCodes()
        {
            return CreateAndExecuteRequest(ResourceNames.ZipCodeService, Method.Get, Scope.Read);
        }

        public RestResponse AddZipCodes(object codes)
        {
            return CreateAndExecuteRequest(ResourceNames.ExpandZipCodeService, Method.Post, Scope.Write, codes);
        }
    }
}
