using System.Numerics;

namespace MEVSharp.Application.Models
{
    public static class Utils
    {
        public static decimal ConvertWeiToEth(BigInteger weiAmount)
        {
            decimal ethAmount = (decimal)weiAmount / 1000000000000000000m;
            return Math.Round(ethAmount, 5);
        }

        public static decimal ConvertWeiToEth(decimal weiAmount)
        {
            decimal ethAmount = (decimal)weiAmount / 1000000000000000000m;
            return Math.Round(ethAmount, 5);
        }

        public static string Remove0x(string hexString)
        {
            if (hexString.StartsWith("0x"))
            {
                hexString = hexString.Substring(2);
            }
            return hexString;
        }
    }
}
