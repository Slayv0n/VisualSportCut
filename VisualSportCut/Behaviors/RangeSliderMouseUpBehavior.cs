using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace VisualSportCut.Presentation.Behaviors
{
    public static class RangeSliderMouseUpBehavior
    {
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof(ICommand),
                typeof(RangeSliderMouseUpBehavior),
                new PropertyMetadata(null, OnMouseUpCommandChanged));

        public static void SetMouseUpCommand(DependencyObject obj, ICommand value)
            => obj.SetValue(MouseUpCommandProperty, value);

        public static ICommand GetMouseUpCommand(DependencyObject obj)
            => (ICommand)obj.GetValue(MouseUpCommandProperty);

        private static void OnMouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RangeSlider slider)
            {
                slider.PreviewMouseLeftButtonUp -= Slider_MouseUp;
                if (e.NewValue is ICommand)
                    slider.PreviewMouseLeftButtonUp += Slider_MouseUp;
            }
        }

        private static void Slider_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is RangeSlider slider &&
                GetMouseUpCommand(slider) is ICommand command)
            {
                command.Execute(null);
            }
        }
    }
}
