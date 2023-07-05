namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Web.Http;
    using Resources;

    public class ResourcesController : ApiController
    {
        public string Get()
        {
            var resourceManager = new ResourceManager(typeof(Lang)) { IgnoreCase = true };

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var dictionary = resourceSet.Cast<DictionaryEntry>();
            var items = dictionary.Select(dict => $"\"{dict.Key}\" : \"{string.Join(",", dict.Value)}\"");
            var jsonString = "{" + string.Join(",", items) + "}";

            return jsonString;
        }
    }
}