using Monolith2LogicApps3.Domain;
using System;

namespace Monolith2LogicApps3.Application.CsvConversion
{
    public class TypeACsvLineConverter : ICsvLineConverter<TypeA>
    {
        public TypeA? Convert(int row, string[] values)
        {
            if (row < 1) //Skip header row
                return null;

            try
            {
                var id = int.Parse(values[0]);
                var field1 = int.Parse(values[1]);
                if (field1 < 5)
                    throw new CsvConversionException("Field1 must have value higher than 5");
                var field2 = int.Parse(values[2]);
                var result = new TypeA
                {
                    Id = id,
                    Field1 = field1,
                    Field2 = field2
                };
                return result;
            }
            catch(Exception ex)
            {
                throw new CsvConversionException("Couldn't parse to expected values", ex);
            }
        }
    }
}