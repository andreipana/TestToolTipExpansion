using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace TestToolTipExpansion
{
	public static class ToolTipExtensions
	{
		public static bool GetIsExpandable(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsExpandableProperty);
		}
		public static void SetIsExpandable(DependencyObject obj, bool value)
		{
			obj.SetValue(IsExpandableProperty, value);
		}
		public static readonly DependencyProperty IsExpandableProperty =
			DependencyProperty.RegisterAttached("IsExpandable", typeof(bool), typeof(ToolTipExtensions), new PropertyMetadata(false, OnIsExpandableChanged));


		public static bool GetIsExpanded(DependencyObject obj)
		{
			return (bool)obj.GetValue(IsExpandedProperty);
		}
		public static void SetIsExpanded(DependencyObject obj, bool value)
		{
			obj.SetValue(IsExpandedProperty, value);
		}
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(ToolTipExtensions), new PropertyMetadata(false, OnIsExpandedChanged));

		private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Console.WriteLine("IsExpanded: " + e.NewValue);
		}

		private static void OnIsExpandableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var tooltip = (ToolTip)d;
			bool isExpandable = true.Equals(e.NewValue);

			if (isExpandable)
				tooltip.IsVisibleChanged += Tooltip_IsVisibleChanged;
			else
				tooltip.IsVisibleChanged -= Tooltip_IsVisibleChanged;
		}

		private static void Tooltip_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var isVisible = e.NewValue?.Equals(true) ?? false;
			var tooltip = (ToolTip)sender;

			if (isVisible)
			{
				CurrentToolTip = tooltip;
				CurrentToolTip?.SetValue(IsExpandedProperty, false);
				StartPolling();
			}
			else
			{
				CurrentToolTip = null;
				StopPolling();
			}
		}

		private static DispatcherTimer Timer;
		private static ToolTip CurrentToolTip;

		private static void StartPolling()
		{
			Timer = new DispatcherTimer();
			Timer.Interval = TimeSpan.FromMilliseconds(50);
			Timer.Tick += Timer_Tick;
			Timer.Start();
		}

		private static void Timer_Tick(object sender, EventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control)
			{
				CurrentToolTip?.SetValue(IsExpandedProperty, true);
				StopPolling();
			}
		}

		private static void StopPolling()
		{
			Timer?.Stop();
			Timer = null;
		}
	}
}
