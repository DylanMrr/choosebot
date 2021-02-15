using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChooseBot.Controllers
{
    [Serializable]
    public class Updates
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("object")]
        public JObject Object { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }
    }

    [Serializable]
    public class Object
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("date")]
        public long Date { get; set; }

        [JsonProperty("out")]
        public long Out { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("read_state")]
        public long ReadState { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("body")]
        public string Body { get; set; }

        [JsonProperty("owner_ids")]
        public IEnumerable<long> OwnerIds { get; set; }
    }
}
