using Monolith2LogicApps3.Domain;
using System;

namespace Monolith2LogicApps3.Application.CsvConversion
{
    public class TypeBCsvLineConverter : ICsvLineConverter<TypeB>
    {
        public TypeB? Convert(int row, string[] values)
        {
            if (row < 2) //Skip 2 header rows
                return null;

            try
            {
                var field1 = int.Parse(values[0]);
                var field2 = int.Parse(values[1]);
                var field3 = int.Parse(values[2]);
                var result = new TypeB
                {
                    Field1 = field1,
                    Field2 = field2,
                    Field3 = field3
                };
                return result;
            }
            catch (Exception ex)
            {
                throw new CsvConversionException("Couldn't parse to expected values", ex);
            }
        }
    }
}