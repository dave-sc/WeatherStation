using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace LoRaWeatherStation.UserInterface.Dashboard
{
    public class WidgetContainer : HeaderedContentControl
    {
        public static readonly StyledProperty<IImage> HeaderIconProperty =
            AvaloniaProperty.Register<WidgetContainer, IImage>(nameof(HeaderIcon));

        public IImage HeaderIcon
        {
            get { return GetValue(HeaderIconProperty); }
            set { SetValue(HeaderIconProperty, value); }
        }

        /// <summary>
        /// Defines the <see cref="BoxShadow"/> property.
        /// </summary>
        public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
            AvaloniaProperty.Register<WidgetContainer, BoxShadows>(nameof(BoxShadow));
        
        /// <summary>
        /// Gets or sets the box shadow effect parameters
        /// </summary>
        public BoxShadows BoxShadow
        {
            get => GetValue(BoxShadowProperty);
            set => SetValue(BoxShadowProperty, value);
        }
    }
}