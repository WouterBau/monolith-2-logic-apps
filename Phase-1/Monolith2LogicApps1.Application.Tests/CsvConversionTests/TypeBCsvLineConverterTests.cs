using Monolith2LogicApps1.Application.CsvConversion;
using Monolith2LogicApps1.Domain;
using System.Collections.Generic;
using Xunit;

namespace Monolith2LogicApps1.Tests;
public class TypeBCsvLineConverterTests
{
    private readonly TypeBCsvLineConverter converter = new TypeBCsvLineConverter();

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void SkippedRows(int row)
    {
        var actual = converter.Convert(row, new string[] { });
        Assert.Null(actual);
    }

    [Theory]
    [InlineData(2, new string[] { })]
    [InlineData(2, new string[] { "A" })]
    [InlineData(2, new string[] { "A", "A" })]
    [InlineData(2, new string[] { "6", "A" })]
    [InlineData(2, new string[] { "A", "1" })]
    [InlineData(2, new string[] { "1", "1", "A" })]
    public void FailingRows(int row, string[] values)
    {
        Assert.Throws<CsvConversionException>(() => converter.Convert(row, values));
    }

    public static IEnumerable<object[]> SuccessValues() => new object[][]
    {
        new object[]
        {
            2, new string[] { "6", "1", "0" }, new TypeB { Field1 = 6, Field2 = 1, Field3 = 0 }
        },
        new object[]
        {
            2, new string[] { "6", "-1", "3" }, new TypeB { Field1 = 6, Field2 = -1, Field3 = 3 }
        },
    };

    [Theory]
    [MemberData(nameof(SuccessValues))]
    public void SucceedingRows(int row, string[] values, TypeB expected)
    {
        var actual = converter.Convert(row, values);
        Assert.NotNull(actual);
        Assert.Equal(expected.Field1, actual.Field1);
        Assert.Equal(expected.Field2, actual.Field2);
        Assert.Equal(expected.Field3, actual.Field3);
    }
}