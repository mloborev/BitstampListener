using Slack.Webhooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitstampListener.Modules
{
    public class SlackIntegrator
    {
        public void SendMessage(SlackClient slackClient, string text)
        {
            var slackMessage = new SlackMessage
            {
                Text = text,
                IconEmoji = Emoji.Ghost,
                Username = "xxx"
            };
            var result = slackClient.Post(slackMessage);
        }
    }
}
