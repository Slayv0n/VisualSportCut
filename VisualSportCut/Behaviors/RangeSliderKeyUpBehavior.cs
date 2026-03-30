using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace VisualSportCut.Presentation.Behaviors
{
    public class RangeSliderKeyUpBehavior
    {
        public static readonly DependencyProperty KeyUpCommandProperty =
           DependencyProperty.RegisterAttached(
               "KeyUpCommand",
               typeof(ICommand),
               typeof(RangeSliderKeyUpBehavior),
               new PropertyMetadata(null, OnKeyUpCommandChanged));

        public static void SetKeyUpCommand(DependencyObject obj, ICommand value)
            => obj.SetValue(KeyUpCommandProperty, value);

        public static ICommand GetKeyUpCommand(DependencyObject obj)
            => (ICommand)obj.GetValue(KeyUpCommandProperty);

        private static void OnKeyUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RangeSlider slider)
            {
                slider.KeyUp -= Slider_KeyUp;
                if (e.NewValue is ICommand)
                    slider.KeyUp += Slider_KeyUp;
            }
        }

        private static void Slider_KeyUp(object sender, KeyEventArgs e)
        {
            if (sender is RangeSlider slider &&
                GetKeyUpCommand(slider) is ICommand command)
            {
                command.Execute(null);
            }
        }
    }
}
