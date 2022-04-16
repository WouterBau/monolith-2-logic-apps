using Monolith2LogicApps0.Application.CsvConversion;
using Monolith2LogicApps0.Domain;
using System.Collections.Generic;
using Xunit;

namespace Monolith2LogicApps0.Tests;
public class TypeACsvLineConverterTests
{
    private readonly TypeACsvLineConverter converter = new TypeACsvLineConverter();

    [Theory]
    [InlineData(0)]
    public void SkippedRows(int row)
    {
        var actual = converter.Convert(row, new string[] { });
        Assert.Null(actual);
    }

    [Theory]
    [InlineData(1, new string[] { })]
    [InlineData(1, new string[] { "A" })]
    [InlineData(1, new string[] { "A", "A" })]
    [InlineData(1, new string[] { "A", "A", "A" })]
    [InlineData(1, new string[] { "A", "5", "A" })]
    [InlineData(1, new string[] { "1", "5", "A" })]
    [InlineData(1, new string[] { "1", "A", "1" })]
    [InlineData(1, new string[] { "1", "4", "1" })]
    public void FailingRows(int row, string[] values)
    {
        Assert.Throws<CsvConversionException>(() => converter.Convert(row, values));
    }

    public static IEnumerable<object[]> SuccessValues() => new object[][]
    {
        new object[]
        {
            1, new string[] { "1", "6", "1" }, new TypeA { Id = 1, Field1 = 6, Field2 = 1 }
        },
        new object[]
        {
            1, new string[] { "1", "5", "-1" }, new TypeA { Id = 1, Field1 = 5, Field2 = -1 }
        },
    };

    [Theory]
    [MemberData(nameof(SuccessValues))]
    public void SucceedingRows(int row, string[] values, TypeA expected)
    {
        var actual = converter.Convert(row, values);
        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Field1, actual.Field1);
        Assert.Equal(expected.Field2, actual.Field2);
    }
}