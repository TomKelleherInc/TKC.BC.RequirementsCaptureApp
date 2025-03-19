using SlackAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace Senda.Requirements.Capture.UI.Common
{
    public static class Slack
    {

        private const string SlackOAuthToken = "xoxp-5013524389-538300972564-597560970470-f9d1c680f46773614066a99a81a03943";
        /*
            private const string SlackApiUri = "https://hooks.slack.com/services/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";
            private const string SlackAppId = "AHJDBDE2H";
            Slack Client ID: 5013524389.596453456085
            Slack Client Secret: f076d618297d18aa4db92d2b68c0f20f
            Slack Signing Secret:  0edf4fb8d12fe941f9ae8c16ad02ff2a
         */

        public enum SlackEmoji { collision, okay }

        public static void Send(string channel, string subject, string body, SlackEmoji emoji)
        {
            try
            {
                string emojiString = ":ok_hand::skin-tone-3:";
                switch(emoji)
                {
                    case SlackEmoji.collision:
                        emojiString = ":boom:";
                        break;

                    default:
                        emojiString = ":ok_hand::skin-tone-3:";
                        break;
                }    

                SlackClient client = new SlackClient(SlackOAuthToken);

                Attachment attach = new SlackAPI.Attachment();
                attach.title = subject;
                attach.text = body;

                //Block block = new Block()
                //{
                //    type = TextTypes.PlainText,
                //    text = new Text() { text = "This is the block text", type = TextTypes.PlainText },
                //    title = new Text() { text = "This is the block title", type = TextTypes.PlainText },
                //    elements = new Element[] { new Element() { value = "block element value" } }
                //};


                Block[] blocks = null; // new Block[] { block };

                client.PostMessage(CallbackResponse, channel, "DevOps UAT Messaging", null, null, false, blocks,  
                    attachments: new[] { attach },
                    icon_emoji: emojiString);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private static void CallbackResponse(PostMessageResponse response)
        {
            // process response from API call
            if (response.ok)
            {
                Console.WriteLine("Message sent successfully");
            }
            else
            {
                Console.WriteLine("Message sending failed. error: " + response.error);
            }
        }


    }
}
