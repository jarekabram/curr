using System;

namespace curr.Models
{
    public class PopularCurrency
    {
        //ID, DATE, CURRENCY_CODE, CURRENCY_NAME,BUY_RATE, SELL_RATE, CONVERSION
        public PopularCurrency(string id, DateTime date, string currencyCode, string currencyName, double buyRate, double sellRate, int conversion)
        {
            Id = id;
            Date = date;
            CurrencyCode = currencyCode;
            CurrencyName = currencyName;
            BuyRate = buyRate;
            SellRate = sellRate;
            Conversion = conversion;
        }
        public string Id{ get; set; }
        public DateTime Date{ get; set; }
        public string CurrencyCode{ get; set; }
        public string CurrencyName{ get; set; }
        public double BuyRate{ get; set; }
        public double SellRate{ get; set; }
        public int Conversion { get; set; }


    }
}