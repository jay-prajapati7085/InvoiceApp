namespace InvoiceApp.NewFolder
{
    public static class NumberToWordsConverter
    {
        private static readonly string[] Units = { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten",
        "Eleven", "Twelve", "Thirteen", "Fourteen","Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };

        private static readonly string[] Tens = { "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

        public static string Convert(decimal number)
        {
            if (number == 0)
                return "Zero";

            var parts = number.ToString("F2").Split('.');
            long integerPart = long.Parse(parts[0]);
            int decimalPart = int.Parse(parts[1]);

            string words = $"{ConvertNumber(integerPart)} Rupees";

            if (decimalPart > 0)
            {
                words += $" and {ConvertNumber(decimalPart)} Paise";
            }

            return words + " Only";
        }

        private static string ConvertNumber(long number)
        {
            if (number < 20)
                return Units[number];

            if (number < 100)
                return Tens[number / 10] + (number % 10 > 0 ? " " + Units[number % 10] : "");

            if (number < 1000)
                return Units[number / 100] + " Hundred" + (number % 100 > 0 ? " and " + ConvertNumber(number % 100) : "");

            if (number < 100000)
                return ConvertNumber(number / 1000) + " Thousand" + (number % 1000 > 0 ? " " + ConvertNumber(number % 1000) : "");

            if (number < 10000000)
                return ConvertNumber(number / 100000) + " Lakh" + (number % 100000 > 0 ? " " + ConvertNumber(number % 100000) : "");

            return ConvertNumber(number / 10000000) + " Crore" + (number % 10000000 > 0 ? " " + ConvertNumber(number % 10000000) : "");
        }
    }

}
