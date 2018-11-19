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
        /*        http://www.nbp.pl/kursy/xml/dir.txt          */
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
            // Helper.TraceMessage(input + " | " + dateTime.Year+"/"+dateTime.Month+"/"+dateTime.Day);
            Content.Add(input, dateTime);
        }
        async Task<string> Load(string input)
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
                if(elem.Key.StartsWith('c'))
                {
                    // Helper.TraceMessage(elem.Key);
                    XmlDocument doc = new XmlDocument();
                    string website = Load(elem.Key).Result;
                    doc.LoadXml(website);
                    XmlNodeList position = doc.SelectNodes("/tabela_kursow/pozycja");
                    foreach (XmlNode xmlNode in position)
                    {
                        string currCode = xmlNode.SelectSingleNode("kod_waluty").InnerText;
                        string currName = xmlNode.SelectSingleNode("nazwa_waluty").InnerText;
                        double buyRate = double.Parse(xmlNode.SelectSingleNode("kurs_kupna").InnerText);
                        double sellRate = double.Parse(xmlNode.SelectSingleNode("kurs_sprzedazy").InnerText);
                        int conversion = int.Parse(xmlNode.SelectSingleNode("przelicznik").InnerText);
                        PopularCurrency pc = new PopularCurrency(elem.Key, elem.Value, currCode, currName, buyRate, sellRate, conversion);
                    
                        System.Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("nazwa_waluty").InnerText);
                        System.Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("przelicznik").InnerText);
                        System.Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kod_waluty").InnerText);
                        System.Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kurs_kupna").InnerText);
                        System.Console.WriteLine("list.InnerText: "+xmlNode.SelectSingleNode("kurs_sprzedazy").InnerText);
                        
                        dc.Insert(pc);
                    }
                }
            }
            dc.CloseConnectionToDatabase();
           
        }
    }
}