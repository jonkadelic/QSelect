﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LibQuakePackageManager.Providers
{
    public class QuaddictedPackageProvider : IProvider<Package>
    {
        #region Variables
        public string databaseUrl = "https://www.quaddicted.com/reviews/quaddicted_database.xml";
        #endregion

        #region Properties
        public List<Package> Items { get; } = new List<Package>();
        #endregion

        #region Methods
        public Package GetItem(string id)
        {
            try
            {
                return Items.First(x => x.Id == id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task RefreshAsync()
        {
            string databaseText;
            using (WebClient client = new WebClient())
            {
                databaseText = await client.DownloadStringTaskAsync(new Uri(databaseUrl));
            }

            Items.Clear();

            XmlDocument document = new XmlDocument();
            document.LoadXml(databaseText);

            // Initialise database items
            if (document["files"] != null)
            {
                await Task.Run(() =>
                {
                    XmlNode files = document["files"];

                    foreach (XmlNode node in files.ChildNodes)
                    {
                        if (node.Name == "file")
                        {
                            Dictionary<string, string> attributes;
                            List<string> dependencies = new List<string>() { "id1" };

                            // Determine rating
                            string rating = null;
                            if (node.Attributes["rating"] != null)
                            {
                                string r = node.Attributes["rating"].InnerText;
                                bool success = int.TryParse(r, out int rint);
                                if (!success) rating = "Unrated";
                                else rating = $"{new string('★', rint)}{new string('☆', 5 - rint)}";
                            }

                            // Determine download size
                            string downloadSize = null;
                            if (node["size"] != null)
                            {
                                uint dlSize = uint.Parse(node["size"]?.InnerText);
                                if (dlSize < 1024)
                                {
                                    downloadSize = $"{dlSize}KB";
                                }
                                else
                                {
                                    downloadSize = $"{(float)dlSize / 1024.0f:#.##}KB";
                                }
                            }

                            // Determine type
                            string type = node.Attributes["type"]?.InnerText switch
                            {
                                "1" => "Single BSP Files",
                                "2" => "Partial Conversion",
                                "4" => "Speedmapping",
                                "5" => "Miscellaneous Files",
                                _ => "Other"
                            };

                            // Initialise attribute list
                            attributes = new Dictionary<string, string>()
                            {
                                { "Title", node["title"]?.InnerText },
                                { "Author", node["author"]?.InnerText },
                                { "Description", node["description"]?.InnerText },
                                { "Release Date", node["date"]?.InnerText },
                                { "Category", type },
                                { "Download Size", downloadSize },
                                { "Rating", rating },
                                { "Screenshot", $"https://www.quaddicted.com/reviews/screenshots/{node.Attributes["id"]?.InnerText}.jpg" }
                            };

                            // Initialise dependency list
                            if (node["techinfo"] != null)
                            {
                                if (node["techinfo"]["requirements"] != null)
                                {
                                    foreach (var file in node["techinfo"]["requirements"])
                                    {
                                        dependencies.Add((file as XmlNode).Attributes["id"]?.InnerText);
                                    }
                                }
                            }

                            Package item = new Package
                            (
                                node.Attributes["id"]?.InnerText,
                                attributes,
                                node["md5sum"]?.InnerText,
                                node["techinfo"]?["zipbasedir"]?.InnerText,
                                $"https://www.quaddicted.com/filebase/{node.Attributes["id"]?.InnerText}.zip",
                                dependencies
                            );

                            Items.Add(item);
                        }
                    }
                });
            }
        }
        #endregion
    }
}
