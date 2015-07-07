// ----------------------------------------------------------------------
// <copyright file="ResourcesController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/2/2015 2:50:44 PM">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections;
    using System.Globalization;
    using System.Linq;
    using System.Resources;
    using System.Web.Http;
    using SMS.EPA.Resources;

    /// <summary>
    /// The resources controller.
    /// </summary>
    public class ResourcesController : ApiController
    {
        /// <summary>
        /// Function to get resource by language.
        /// </summary>
        /// <param name="lang">Language input.</param>
        /// <returns>String resource JSON format.</returns>
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
