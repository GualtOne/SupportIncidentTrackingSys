using System.Text.RegularExpressions;
using System.Linq;

namespace SupportIncidentTrackingSys
{
    public class Hash
    {
        private const int M = 69;

        public static string MakeHash(string value)
        {
            MatchCollection matches = Regex.Matches(value, @"\d+");
            List<int> numbers = [];
            numbers.AddRange(from Match match in matches
                             select int.Parse(match.Value));
            decimal temp = 0;
            foreach (int num in numbers)
            {
                temp += num;
            }
            decimal v = (value.Length + temp) / M;
            return v.ToString();
        }
    }
}