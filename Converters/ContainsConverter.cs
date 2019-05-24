using System.Collections;
using System.Globalization;

namespace ShiftCheck.Converters;

public class ContainsConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is IList list && parameter != null)
		{
			return list.Contains(parameter);
		}
		return false;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
