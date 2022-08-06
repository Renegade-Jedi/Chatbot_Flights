using System;
using System.Security.Cryptography;
using System.Text;

namespace Chatbot_Flights.Models
{
    public class FakeDataGenerator
    {

        public static double CalculateTripCost(string FlightClass, string AirPortFrom, string AirPortTo)
        {
            double cost = (Math.Abs(AirPortFrom.Length - AirPortTo.Length) + 1) * 100;

            switch (FlightClass)
            {
                case "Standard":
                    cost *= 1.1;
                    break;
                case "Business":
                    cost *= 1.5;
                    break;
                case "Premium":
                    cost *= 2.0;
                    break;
                default:
                    break;
            }
            return cost;
        }

        public static double CalculateTripCostTwoWay(double cost)
        {
            cost *= 1.8;
            return cost;
        }


        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }
    }   
}

