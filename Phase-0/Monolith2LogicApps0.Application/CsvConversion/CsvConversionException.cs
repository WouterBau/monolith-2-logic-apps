using System;

namespace Monolith2LogicApps0.Application.CsvConversion
{
    public class CsvConversionException : Exception
    {
        public CsvConversionException(string message) : base(message)
        {

        }
        public CsvConversionException(string message, Exception ex) : base(message, ex)
        {

        }
    }
}
