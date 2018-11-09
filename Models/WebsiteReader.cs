using System;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace curr.Models
{
    public class WebsiteReader
    {
        /*        http://www.nbp.pl/kursy/xml/dir.txt          */
        Dictionary<string, DateTime> Content; // file, date
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
         /*

            'xnnnzrrmmdd.xml' 
            x – litera określająca typ tabeli:
                a - tabela kursów średnich walut obcych;
                b - tabela kursów średnich walut niewymienialnych;
                c - tabela kursów kupna i sprzedaży;
                h - tabela kursów jednostek rozliczeniowych.
            nnn – trzyznakowy (liczbowy) numer tabeli w roku; 
            z – litera ‘z’ (element stały) 
            rrmmdd – data publikacji/obowiązywania tabeli w formacie (bez odstępów): dwie ostatnie cyfry numeru roku, dwie cyfry numeru miesiąca oraz dwie cyfry numeru dnia. 
            */
        private void parseDate(string input)
        {
            /*c214z181105*/
            
            string yearstr = "20" + input.Substring(5, 2);
            int year = Int32.Parse(yearstr);
            int month = Int32.Parse(input.Substring(7, 2));
            int day = Int32.Parse(input.Substring(9, 2));
            
            DateTime dateTime = new DateTime(year, month, day);
            Console.Write(input + " | " + dateTime.Year+"/"+dateTime.Month+"/"+dateTime.Day);
            Content.Add(input, dateTime);

            Console.WriteLine();
        }
    }
}