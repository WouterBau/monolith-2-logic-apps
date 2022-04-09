namespace Monolith2LogicApps0.Application.CsvConversion
{
    public interface ICsvLineConverter<T> where T : class
    {
        T? Convert(int row, string[] values);
    }
}
