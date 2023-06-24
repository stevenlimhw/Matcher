namespace TradingEngineServer.Matcher
{
    public class DataParser
    {
        private readonly char delimiter;

        public DataParser(char delimiter = ',')
        {
            this.delimiter = delimiter;
        }

        public List<List<string>> ParseCsv(string filePath)
        {
            List<List<string>> csvData = new List<List<string>>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                reader.ReadLine(); // skip the column headers
                while ((line = reader.ReadLine()) != null)
                {
                    List<string> row = ParseCsvLine(line);
                    csvData.Add(row);
                }
            }

            return csvData;
        }

        private List<string> ParseCsvLine(string line)
        {
            List<string> fields = new List<string>();

            int currentIndex = 0;
            int lineLength = line.Length;

            while (currentIndex < lineLength)
            {
                string fieldValue;

                if (line[currentIndex] == '"')
                {
                    int nextQuoteIndex = line.IndexOf('"', currentIndex + 1);

                    while (nextQuoteIndex > -1 && nextQuoteIndex < lineLength - 1 && line[nextQuoteIndex + 1] == '"')
                    {
                        nextQuoteIndex = line.IndexOf('"', nextQuoteIndex + 2);
                    }

                    if (nextQuoteIndex == -1)
                    {
                        // Invalid CSV format - mismatched quotes
                        throw new FormatException("Invalid CSV format");
                    }

                    fieldValue = line.Substring(currentIndex + 1, nextQuoteIndex - currentIndex - 1);
                    fieldValue = fieldValue.Replace("\"\"", "\"");

                    currentIndex = nextQuoteIndex + 2;
                }
                else
                {
                    int nextDelimiterIndex = line.IndexOf(delimiter, currentIndex);

                    if (nextDelimiterIndex == -1)
                    {
                        nextDelimiterIndex = lineLength;
                    }

                    fieldValue = line.Substring(currentIndex, nextDelimiterIndex - currentIndex);
                    currentIndex = nextDelimiterIndex + 1;
                }

                fields.Add(fieldValue);
            }

            return fields;
        }
    }

}