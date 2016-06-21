namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Web.Http;
    using SMS.EPA.Resources;

    public class ResourcesController : ApiController
    {
        public string Get(string lang)
        {
            ResourceManager resourceManager = null;
            switch (lang)
            {
                case "en":
                    resourceManager = new ResourceManager(typeof(Lang)) { IgnoreCase = true };
                    break;

                default:
                    resourceManager = new ResourceManager(typeof(Lang)) { IgnoreCase = true };
                    break;
            }

            ResourceSet resourceSet = resourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            var dictionary = resourceSet.Cast<DictionaryEntry>();
            var items = dictionary.Select(dict => string.Format("\"{0}\" : \"{1}\"", dict.Key, string.Join(",", dict.Value)));
            var jsonString = "{" + string.Join(",", items) + "}";

            return jsonString;
        }
    }
}