using System;
using System.Collections.Generic;
using System.Text;

namespace ScmBackup.Hosters.XpDev
{
    using Newtonsoft.Json;

    class XpDevErrorMessageResponse
    {

        [JsonProperty("message")]
        public string ErrorMessage { get; set; }

        public static XpDevErrorMessageResponse FromJson(string json)
        {
            return JsonConvert.DeserializeObject<XpDevErrorMessageResponse>(json, XpDevJsonHelpers.Settings);
        }
    }
}
