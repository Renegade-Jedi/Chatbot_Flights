using Microsoft.Recognizers.Text.DataTypes.TimexExpression;
using System;
using System.Linq;

namespace Chatbot_Flights.ProjectClass
{
    public class DataValidator
    {
        private static bool isHistory;
        private static bool isBigger;

        public static bool IsValidDate(string DateStr)
        {
            DateStr = DateStr.Trim();
            DateTime tempDate = new DateTime().Date;

            return DateTime.TryParse(DateStr, new System.Globalization.CultureInfo("pl-PL"), System.Globalization.DateTimeStyles.None, out tempDate);
        }

        public static bool IsHistoricalDate(string DateStr)
        {
            return isHistory = Convert.ToDateTime(DateStr) < DateTime.Now;

        }

        public static bool CompareTwoDates(string DateStr, string DateStrTwo)
        {
            return isBigger = Convert.ToDateTime(DateStr) < Convert.ToDateTime(DateStrTwo);

        }

        public static bool IsNumber(string Number)
        {
            try
            {
                if (int.Parse(Number.ToString()).GetType().Equals(typeof(int)))
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
