using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace gorsel_programlam_odev2
{
    internal class RssConverter
    {
        public static string ConvertRssToJson(string rssUrl)
        {
            XDocument rssFeed = XDocument.Load(rssUrl);

            var json = new JObject(
                new JProperty("items",
                    new JArray(
                        from item in rssFeed.Descendants("item")
                        select new JObject(
                            new JProperty("title", item.Element("title").Value),
                            new JProperty("link", item.Element("link").Value)
                        )
                    )
                )
            );

            return json.ToString();
           Console.Read();
        }
       
    }
    
}
