using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Visuals.Media.Imaging;
using OxyPlot;
using OxyPlot.Avalonia;
using HorizontalAlignment = Avalonia.Layout.HorizontalAlignment;
using VerticalAlignment = Avalonia.Layout.VerticalAlignment;

namespace LoRaWeatherStation.UserInterface.OxyPlotExtensions
{
    public class ImageAnnotation : Annotation
    {
        public static readonly StyledProperty<IImage> SourceProperty = AvaloniaProperty.Register<ImageAnnotation, IImage>(nameof(Source));
        public static readonly StyledProperty<PlotLength> XProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(X), new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea));
        public static readonly StyledProperty<PlotLength> YProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(Y), new PlotLength(0.5, PlotLengthUnit.RelativeToPlotArea));
        public static readonly StyledProperty<PlotLength> OffsetXProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(OffsetX), new PlotLength(0.0, PlotLengthUnit.ScreenUnits));
        public static readonly StyledProperty<PlotLength> OffsetYProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(OffsetX), new PlotLength(0.0, PlotLengthUnit.ScreenUnits));
        public new static readonly StyledProperty<PlotLength> WidthProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(Width), new PlotLength(double.NaN, PlotLengthUnit.ScreenUnits));
        public new static readonly StyledProperty<PlotLength> HeightProperty = AvaloniaProperty.Register<ImageAnnotation, PlotLength>(nameof(Height), new PlotLength(double.NaN, PlotLengthUnit.ScreenUnits));
        public static readonly StyledProperty<bool> InterpolateProperty = AvaloniaProperty.Register<ImageAnnotation, bool>(nameof(Interpolate), true);

        private OxyImage _image;
        
        static ImageAnnotation()
        {
            SourceProperty.Changed.AddClassHandler<ImageAnnotation>(ImageChanged);
            XProperty.Changed.AddClassHandler<ImageAnnotation>(DataChanged);
            YProperty.Changed.AddClassHandler<ImageAnnotation>(DataChanged);
            OffsetXProperty.Changed.AddClassHandler<ImageAnnotation>(DataChanged);
            OffsetYProperty.Changed.AddClassHandler<ImageAnnotation>(DataChanged);
            WidthProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
            HeightProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
            OpacityProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
            HorizontalAlignmentProperty.OverrideDefaultValue<ImageAnnotation>(HorizontalAlignment.Center);
            HorizontalAlignmentProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
            VerticalAlignmentProperty.OverrideDefaultValue<ImageAnnotation>(VerticalAlignment.Center);
            VerticalAlignmentProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
            InterpolateProperty.Changed.AddClassHandler<ImageAnnotation>(AppearanceChanged);
        }

        public ImageAnnotation()
        {
            InternalAnnotation = new OxyPlot.Annotations.ImageAnnotation();
        }

        public PlotLength X
        {
            get => GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public PlotLength Y
        {
            get => GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public new PlotLength Width
        {
            get => GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

        public new PlotLength Height
        {
            get => GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public PlotLength OffsetX
        {
            get => GetValue(OffsetXProperty);
            set => SetValue(OffsetXProperty, value);
        }

        public PlotLength OffsetY
        {
            get => GetValue(OffsetYProperty);
            set => SetValue(OffsetYProperty, value);
        }

        public bool Interpolate
        {
            get => GetValue(InterpolateProperty);
            set => SetValue(InterpolateProperty, value);
        }

        public IImage Source
        {
            get => GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        
        public override OxyPlot.Annotations.Annotation CreateModel()
        {
            SynchronizeProperties();
            return InternalAnnotation;
        }
        
        public override void SynchronizeProperties()
        {
            base.SynchronizeProperties();
            var internalAnnotation = (OxyPlot.Annotations.ImageAnnotation)InternalAnnotation;
            internalAnnotation.X = Convert(X);
            internalAnnotation.Y = Convert(Y);
            internalAnnotation.OffsetX = Convert(OffsetX);
            internalAnnotation.OffsetY = Convert(OffsetY);
            internalAnnotation.Width = Convert(Width);
            internalAnnotation.Height = Convert(Height);
            internalAnnotation.Opacity = Opacity;
            internalAnnotation.HorizontalAlignment = Convert(HorizontalAlignment);
            internalAnnotation.VerticalAlignment = Convert(VerticalAlignment);
            internalAnnotation.Interpolate = Interpolate;
            internalAnnotation.ImageSource = _image;
        }

        private OxyPlot.PlotLength Convert(PlotLength plotLength) => new OxyPlot.PlotLength(plotLength.Value, plotLength.Unit);

        private OxyPlot.VerticalAlignment Convert(VerticalAlignment verticalAlignment) => verticalAlignment switch
        {
            VerticalAlignment.Stretch => OxyPlot.VerticalAlignment.Middle,
            VerticalAlignment.Top => OxyPlot.VerticalAlignment.Top,
            VerticalAlignment.Center => OxyPlot.VerticalAlignment.Middle,
            VerticalAlignment.Bottom => OxyPlot.VerticalAlignment.Bottom,
            _ => throw new ArgumentOutOfRangeException(nameof(verticalAlignment))
        };

        private OxyPlot.HorizontalAlignment Convert(HorizontalAlignment horizontalAlignment) => horizontalAlignment switch
        {
            HorizontalAlignment.Stretch => OxyPlot.HorizontalAlignment.Center,
            HorizontalAlignment.Left => OxyPlot.HorizontalAlignment.Left,
            HorizontalAlignment.Center => OxyPlot.HorizontalAlignment.Center,
            HorizontalAlignment.Right => OxyPlot.HorizontalAlignment.Right,
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlignment))
        };

        private static void ImageChanged(ImageAnnotation d, AvaloniaPropertyChangedEventArgs e)
        {
            if (d.Source == null)
            {
                d._image = null;
            }
            else
            {
                using (var stream = new MemoryStream())
                {
                    using (var bitmap = new RenderTargetBitmap(new PixelSize((int)d.Source.Size.Width, (int)d.Source.Size.Height)))
                    using (IDrawingContextImpl ctxImpl = bitmap.CreateDrawingContext(null))
                    using (DrawingContext ctx = new DrawingContext(ctxImpl))
                    {
                        d.Source.Draw(ctx, new Rect(d.Source.Size), new Rect(d.Source.Size), BitmapInterpolationMode.Default);
                        bitmap.Save(stream);
                    }

                    stream.Seek(0, SeekOrigin.Begin);
                    d._image = new OxyImage(stream);
                }
            }
            
            AppearanceChanged(d, e);
        }
    }
}