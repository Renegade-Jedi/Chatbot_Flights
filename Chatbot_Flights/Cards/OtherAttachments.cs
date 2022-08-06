using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Chatbot_Flights.Cards
{
    public class OtherAttachments
    {
        public static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\plane.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"Resources\plane.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }
    }
}
