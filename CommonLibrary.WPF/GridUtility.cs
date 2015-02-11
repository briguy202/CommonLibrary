using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace CommonLibrary.WPF {
	public class GridUtility {
		public enum GridRowType {
			NotSet,
			NewRow,
			NewStarRow,
			CurrentRow
		}

		private static Dictionary<string, int> _collection = new Dictionary<string,int>();
		public static readonly DependencyProperty TypeProperty = DependencyProperty.Register("Type", typeof(GridRowType), typeof(FrameworkElement), new PropertyMetadata(GridRowType.NotSet, TypeChanged));

		public static void TypeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			string newValue = e.NewValue.ToString();
			FrameworkElement element = (FrameworkElement)obj;
			if (!(element.Parent is Grid)) { return; }
			Grid parent = (Grid)element.Parent;
			GridRowType type = (GridRowType)obj.GetValue(TypeProperty);

			string group = (!string.IsNullOrEmpty(parent.Name)) ? parent.Name : "default";
			if (!_collection.ContainsKey(group)) {
				_collection.Add(group, -1);
			}

			if (type == GridRowType.NewRow || type == GridRowType.NewStarRow) {
				_collection[group]++;
				parent.RowDefinitions.Add(new RowDefinition() { 
					Height = (type == GridRowType.NewStarRow) ? new GridLength(2, GridUnitType.Star) : GridLength.Auto
				});
			}

			Grid.SetRow(element, _collection[group]);
		}

		public static GridRowType GetType(DependencyObject obj) {
			GridRowType type = GridRowType.NotSet;
			if (!Enum.TryParse<GridRowType>((string)obj.GetValue(TypeProperty), out type)) {
				type = GridRowType.NotSet;
			}
			return type;
		}

		public static void SetType(DependencyObject obj, string value) {
			obj.SetValue(TypeProperty, Enum.Parse(typeof(GridRowType), value));
		}
	}
}
