using System;
using System.Net;
using System.Xml;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace curr.Models
{
    public class WebsiteReader
    {
        Dictionary<string, DateTime> Content;
        public WebsiteReader()
        {
            Content = new Dictionary<string, DateTime>();
        }
        public void DownloadLines(string hostUrl)
        {
            var webRequest = WebRequest.Create(hostUrl);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                while (!reader.EndOfStream)
                {
                    parseDate(reader.ReadLine());
                }
            }
        }
        private void parseDate(string input)
        {
            string yearstr = "20" + input.Substring(5, 2);
            int year = Int32.Parse(yearstr);
            int month = Int32.Parse(input.Substring(7, 2));
            int day = Int32.Parse(input.Substring(9, 2));
            
            DateTime dateTime = new DateTime(year, month, day);
            Helper.TraceMessage(input + " | " + dateTime.Year+"/"+dateTime.Month+"/"+dateTime.Day);

            if(!Content.ContainsKey(input))
            {
                Content.Add(input, dateTime);
            }
            else
            {
                Helper.TraceMessage("Element already inserted into dictionary");
            }
        }
        async Task<string> LoadXmlFromWebsite(string input)
        {
            var hc = new HttpClient();
            string s = await hc.GetStringAsync("http://www.nbp.pl/kursy/xml/"+input+".xml");
            return s;
        }
        public void GatherData()
        {
            DatabaseConnection dc = new DatabaseConnection();
            dc.LoginToDatabase();
            foreach(var elem in Content)
            {
                if(dc.InsertIntoFilesAndDatesTable(elem.Key, elem.Value))
                {
                    if(elem.Key.StartsWith('c'))
                    {
                        Helper.TraceMessage(elem.Key);

                        XmlDocument doc = new XmlDocument();
                        string website = LoadXmlFromWebsite(elem.Key).Result;

                        LoadXmlFromWebsite(elem.Key).Wait();
                        try
                        {
                            doc.LoadXml(website);
                        }
                        catch(XmlException xmlException)
                        {
                            Helper.TraceMessage(xmlException.StackTrace);
                        }
                        XmlNodeList position = doc.SelectNodes("/tabela_kursow/pozycja");
                        foreach (XmlNode xmlNode in position)
                        {
                            string currCode = xmlNode.SelectSingleNode("kod_waluty").InnerText;
                            string currName = xmlNode.SelectSingleNode("nazwa_waluty").InnerText;
                            double buyRate = double.Parse(xmlNode.SelectSingleNode("kurs_kupna").InnerText);
                            double sellRate = double.Parse(xmlNode.SelectSingleNode("kurs_sprzedazy").InnerText);
                            int conversion = int.Parse(xmlNode.SelectSingleNode("przelicznik").InnerText);
                            PopularCurrency pc = new PopularCurrency(elem.Key, elem.Value, currCode, currName, buyRate, sellRate, conversion);
                        
                            Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("nazwa_waluty").InnerText);
                            Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("przelicznik").InnerText);
                            Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kod_waluty").InnerText);
                            Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kurs_kupna").InnerText);
                            Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kurs_sprzedazy").InnerText);
                            
                            dc.InsertIntoCurrencyTable(pc);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Record was already added");
                }
            }
            dc.CloseConnectionToDatabase();
        }
    }
}