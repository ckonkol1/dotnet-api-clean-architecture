using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace PlantTracker.Infrastructure.Converters;

public class DateTimeOffsetPropertyConverter : IPropertyConverter
{
    public static readonly string DateTimeOffsetPersistenceFormatString = "yyyy-MM-ddTHH:mm:ss.ffffzzz";

    public object FromEntry(DynamoDBEntry entry)
    {
        if (entry == null)
        {
            throw new ArgumentNullException(nameof(entry));
        }

        if (entry is not Primitive primitive)
        {
            throw new ArgumentException($"{nameof(entry)} [{entry?.GetType()?.Name}] is not an instance of {nameof(Primitive)}.");
        }

        var dateString = primitive.Value as string;
        if (string.IsNullOrWhiteSpace(dateString))
        {
            throw new ArgumentException($"{nameof(entry)} does not contain a string primitive value.");
        }

        if (DateTimeOffset.TryParse(dateString, out var dateTimeOffset))
        {
            return dateTimeOffset;
        }

        throw new ArgumentException($"{nameof(entry)} primitive string value could not be parsed.");
    }

    public DynamoDBEntry ToEntry(object? value)
    {
        if (value == null)
        {
            return new Primitive();
        }

        if (value is not DateTimeOffset dateTimeOffset)
        {
            throw new ArgumentException($"{nameof(value)} [{value.GetType().Name}] is not an instance of {nameof(DateTimeOffset)}.");
        }

        var entry = new Primitive(dateTimeOffset.ToString(DateTimeOffsetPersistenceFormatString));
        return entry;
    }
}