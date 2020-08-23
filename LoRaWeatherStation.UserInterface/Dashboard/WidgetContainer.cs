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
        /// Defines the <see cref="CornerRadius"/> property.
        /// </summary>
        public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
            AvaloniaProperty.Register<WidgetContainer, CornerRadius>(nameof(CornerRadius));

        /// <summary>
        /// Gets or sets the radius of the border rounded corners.
        /// </summary>
        public CornerRadius CornerRadius
        {
            get { return GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
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