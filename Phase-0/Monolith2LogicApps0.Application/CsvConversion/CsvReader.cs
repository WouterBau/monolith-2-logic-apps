using System.Collections.Generic;
using System.IO;

namespace Monolith2LogicApps0.Application.CsvConversion
{
    public class CsvReader
    {
        public IEnumerable<T> Parse<T>(Stream stream, ICsvLineConverter<T> csvLineConverter) where T : class
        {
            int row = 0;
            var result = new List<T>();
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var splitLine = line.Split(';');
                    var rowResult = csvLineConverter.Convert(row, splitLine);
                    if(rowResult != null)
                        result.Add(rowResult);
                    row++;
                }
            }
            return result;
        }
    }
}
