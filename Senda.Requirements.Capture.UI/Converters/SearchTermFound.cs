using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Senda.Requirements.Capture.UI.Converters
{

	/// <summary>
	/// Used to show/hide the TopicSearch UI items in the left-side list, based on whether their
	/// text value is found in the source document.
	/// </summary>
    class SearchTermFound : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility vis = (Visibility)value;
			
			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is bool)
			{
				if ((bool)value == true)
					return "yes";
				else
					return "no";
			}
			return "no";
		}
	}
}
