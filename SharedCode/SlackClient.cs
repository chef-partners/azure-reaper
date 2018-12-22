using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Azure.Reaper
{
    class SlackClient
    {
        private readonly Uri _webhookUrl;
        private readonly String _username;
        private readonly String _icon_url;
        private readonly String _token;
        private readonly HttpClient _httpClient = new HttpClient();
        private ILogger _log;
        private bool enabled;
        private string slackUserId;

        private ArrayList fields;
        private ArrayList attachments;

        public SlackClient(
            Uri webhookUrl,
            string username,
            string icon_url,
            string token,
            ILogger log,
            bool enabled)
        {
            _webhookUrl = webhookUrl;
            _username = username;
            _icon_url = icon_url;
            _token = token;
            _log = log;
            this.enabled = enabled;
        }

        public async Task<HttpResponseMessage> SendMessageAsync()
        {
            HttpResponseMessage response = null;

            // only send the message if the slackUserId is set and Slack is enabled
            if (!String.IsNullOrEmpty(slackUserId) && enabled)
            {
                // Set the Slack endpoint
                UriBuilder builder =  new UriBuilder("Https://slack.com/api/chef.postMessage");

                // Create the payload to send to slack
                var payload = new
                {
                    text = "",
                    channel = slackUserId,
                    username = _username,
                    icon_url = _icon_url,
                    attachments = attachments
                };

                // Serialize the payload so it can be sent to Slack
                var serialisedPayload = JsonConvert.SerializeObject(payload);

                // set the necessary headers on the client
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);

                // make the call to slack
                response = await _httpClient.PostAsync(builder.ToString(), new StringContent(serialisedPayload, Encoding.UTF8, "application/json"));

                // reset fields and attachments so that the client object can be used again
                fields = new ArrayList();
                attachments = new ArrayList();
            }
            else
            {
                _log.LogDebug("Not sending Slack notification. slackUserId: {0}, enabled: {1}", slackUserId, enabled.ToString());
            }

            return response;
        }

        public async void GetUserIdByEmail(string email)
        {

            // Define the URL that needs to be accessed
            UriBuilder builder = new UriBuilder("https://slack.com/api/users.lookupByEmail");

            // create the query string
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["token"] = _token;
            query["email"] = email;

            // Add the query to the builder
            builder.Query = query.ToString();

            // make the call
            var response = await _httpClient.GetAsync(builder.ToString());
            var result = response.Content.ReadAsStringAsync().Result;

            // Get the id of the user with the email address
            JObject s = JObject.Parse(result);

            if ((bool)s["ok"])
            {
                slackUserId = (string)s["user"]["id"];
            }
        }

        public void AddField(string title, dynamic value)
        {
            var field = new {
                title = title,
                value = value
            };

            // Add to the fields array list
            fields.Add(field);
        }

        public void AddAttachmentItem(string message, string colour)
        {
            var item = new {
                text = message,
                color = colour,
                fields = fields
            };

            // Add the item to the attachments
            attachments.Add(item);
        }
    }
}
