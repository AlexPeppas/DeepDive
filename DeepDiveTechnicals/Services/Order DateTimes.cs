using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DeepDiveTechnicals.Services
{
    public static class Order_DateTimes
    {
        public static List<string> OrderDate()
        {
            var req = new List<string> {  
                "18/10/2021 19:23:44.253809",
                "28/09/2021 11:27:55.549826",
                //"27/09/2021 14:54:30",
                "19/10/2021 10:50:38.698090",
                "27/09/2021 18:22:10.003811",
                "19/10/2021 12:40:40.650103",
                "27/09/2021 17:46:31.327143",
                "19/10/2021 02:50:36.432506",
                "19/10/2021 07:40:37.621033",
                "20/10/2021 00:10:43.670525"};
            
            string dateString = "18/08/2015 06:30:15.006542";
            string format = "dd/MM/yyyy HH:mm:ss.ffffff";
            CultureInfo provider = CultureInfo.InvariantCulture;
            try
            {
                /*req = req.OrderByDescending(it => DateTime.ParseExact(it, format, provider).TimeOfDay)
                   .ThenByDescending
                   (it => (it != null) ?
                   DateTime.ParseExact(it.Substring(0, 10).ToString(), "dd/MM/yyyy", null) :
                   DateTime.MinValue
                   )?.ToList();*/

                req = req.OrderByDescending(it => (it != null) ?
                   DateTime.ParseExact(it.Substring(0, 10).ToString(), "dd/MM/yyyy", null) :
                   DateTime.MinValue
                   )
                   .ThenByDescending
                   (it => DateTime.ParseExact(it, format, provider).TimeOfDay)
                   ?.ToList();

                //DateTime result = DateTime.ParseExact(dateString, format, provider);
                //Console.WriteLine("{0} converts to {1}.", dateString, result.TimeOfDay.ToString());
            }
            catch (Exception ex) { }

            //req = req.OrderByDescending(it=> DateTime.ParseExact(it.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay)
                req = req.OrderByDescending(it => DateTime.ParseExact(it.Substring(0, 19).ToString(), "dd/MM/yyyyThh:mm:ss", null).TimeOfDay)
                .ThenByDescending
                (it => (it != null) ?
                DateTime.ParseExact(it.Substring(0, 10).ToString(), "dd/MM/yyyy", null) :
                DateTime.MinValue
                )?.ToList();
            /*req =req.OrderByDescending
                (it => (it!=null) ?
                DateTime.ParseExact(it.Substring(0,19).ToString(),"dd/MM/yyyyThh:mm:ss",null) : 
                DateTime.MinValue
                )?.ToList();*/
            return req;
        }
        public static List<T> OrderDates<T>(List<T> request) where T : struct
        {
           
            return new List<T>();
        }
        
    }
    
}
