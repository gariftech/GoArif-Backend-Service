using System;
using System.Globalization;
using System.Text.RegularExpressions;

public class NumberConverter
{
    public static double ConvertAbbreviatedNumber(string file)
    {
        string input = Regex.Replace(file, "[^0-9]", "");
        
        if (string.IsNullOrEmpty(input))
        {
            input = "1";
        }

        double result;
        if (double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
        {
            return result;
        }

        input = input.Trim().ToUpperInvariant();

        // Check for the multiplier suffix (K, M, B, etc.)
        if (input.EndsWith(" rb x ditonton"))
        {
            string numberString = input.Replace(" rb x ditonton", "");
            if (double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                return number * 1000;
            }
            else
            {
                throw new ArgumentException("Invalid number format");
            }
        }
        if (input.EndsWith(" jt x ditonton"))
        {
            string numberString = input.Replace(" jt x ditonton", "");
            if (double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture, out double number))
            {
                return number * 1000000;
            }
            else
            {
                throw new ArgumentException("Invalid number format");
            }
        }
        if (input.EndsWith("K"))
        {
            return double.Parse(input.Replace("K", ""), CultureInfo.InvariantCulture) * 1000;
        }
        else if (input.EndsWith("M"))
        {
            return double.Parse(input.Replace("M", ""), CultureInfo.InvariantCulture) * 1000000;
        }
        else if (input.EndsWith("B"))
        {
            return double.Parse(input.Replace("B", ""), CultureInfo.InvariantCulture) * 1000000000;
        }
        // You can add additional cases for other suffixes as needed

        throw new ArgumentException("Invalid input format.");
    }
}