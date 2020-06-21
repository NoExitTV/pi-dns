using Newtonsoft.Json;

namespace Pi.Dns.Function.Notifications.Models
{
    public class TelegramRequest
    {
        [JsonProperty("chat_id")]
        public string Chat_id { get; set; }

        [JsonProperty("parse_mode")]
        public string Parse_mode { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public TelegramRequest(string chatId, string parseMode, string text)
        {
            Chat_id = chatId;
            Parse_mode = parseMode;
            Text = text;
        }
    }
}
