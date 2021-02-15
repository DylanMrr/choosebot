using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChooseBot.Models
{
    [Serializable]
    public class Keyboard
    {
        [JsonProperty("one_time")]
        public bool OneTime { get; set; }

        [JsonProperty("buttons")]
        public List<Button> Buttons { get; set; }
    }

    [Serializable]
    public class Button
    {
        [JsonProperty("action")]
        public Action Action { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }
    }

    [Serializable]
    public class Action
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        
        [JsonProperty("payload")]
        public string Payload { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
