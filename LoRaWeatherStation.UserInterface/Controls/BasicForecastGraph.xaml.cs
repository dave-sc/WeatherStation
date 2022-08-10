using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DynamicData;
using LoRaWeatherStation.DataModel;
using LoRaWeatherStation.UserInterface.Converters;
using LoRaWeatherStation.UserInterface.OxyPlotExtensions;
using LoRaWeatherStation.Utils.Reactive;
using OxyPlot;
using OxyPlot.Avalonia;
using ReactiveUI;

namespace LoRaWeatherStation.UserInterface.Controls
{
    public class BasicForecastGraph : UserControl, IActivatableView
    {
        private Plot Plot => this.FindControl<Plot>("Plot");
        private LinearAxis TempAxis => this.FindControl<LinearAxis>("TempAxis");
        private LinearAxis RainAxis => this.FindControl<LinearAxis>("RainAxis");
        private DateTimeAxis TimeAxis => this.FindControl<DateTimeAxis>("TimeAxis");
        private LineSeries TempSeries => this.FindControl<LineSeries>("TempSeries");
        private LineSeries RainSeries => this.FindControl<LineSeries>("RainSeries");
        
        public BasicForecastGraph()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                var forecast = this.WhenAnyValue(x => x.Forecast)
                    .Select(data => data ?? new ForecastData[0])
                    .Select(data => data.OrderBy(y => y.Time).ToArray())
                    .Select(data => data.Where(y => y.Time <= (data.FirstOrDefault()?.Time.Date ?? DateTime.MinValue.Date).AddDays(1)).ToArray());

                forecast
                    .Select(data => data.Select(item => OxyPlot.Axes.DateTimeAxis.CreateDataPoint(item.Time, item.Temperature)))
                    .BindTo(TempSeries, x => x.Items)
                    .DisposeWith(disposables);
                
                forecast
                    .Select(data => data.Select(item => OxyPlot.Axes.DateTimeAxis.CreateDataPoint(item.Time, item.Precipitation)))
                    .BindTo(RainSeries, x => x.Items)
                    .DisposeWith(disposables);

                forecast
                    .Select(data => data
                        .Where(item => item.Time.TimeOfDay.TotalHours % 3 == 0)
                        .Select(item => new ImageAnnotation()
                        {
                            Source = WeatherTypeToImageConverter.Convert(item.Weather),
                            // Warning: OxyPlot does not properly position ImageAnnotations that have one coordinate of type Data when the other one isn't. So this does not work: 
                            // X = new PlotLength(OxyPlot.Axes.DateTimeAxis.ToDouble(y.Time), PlotLengthUnit.Data),
                            // so calculate the X-coordinate locally instead, which is relatively cheap as the data is already ordered and an array.
                            X = new OxyPlotExtensions.PlotLength((item.Time - data.First().Time).TotalHours / (data.Last().Time - data.First().Time).TotalHours, PlotLengthUnit.RelativeToPlotArea),
                            Y = new OxyPlotExtensions.PlotLength(12, PlotLengthUnit.ScreenUnits),
                            Width = new OxyPlotExtensions.PlotLength(24, PlotLengthUnit.ScreenUnits),
                        })
                    )
                    .Subscribe(annotations =>
                    {
                        // Annotations is readonly
                        Plot.Annotations.Clear();
                        Plot.Annotations.AddRange(annotations);
                    })
                    .DisposeWith(disposables);

                var tempRange = forecast
                    .Select(data => data.Any() ? (min: data.Min(item => item.Temperature), max: data.Max(item => item.Temperature)) : (min: 0, max: 20))
                    .Select((minTemp, maxTemp) => (min: Math.Floor(minTemp / 5) * 5, max: Math.Ceiling(maxTemp / 5) * 5));
                
                tempRange
                    .Select(range => range.min)
                    .BindTo(TempAxis, x => x.Minimum)
                    .DisposeWith(disposables);
                
                tempRange
                    .Select(range => range.max)
                    .BindTo(TempAxis, x => x.Maximum)
                    .DisposeWith(disposables);
                
                var timeRange = forecast
                    .Select(data => data.FirstOrDefault()?.Time.Date ?? DateTime.MinValue.Date)
                    .Select(x => (min: x, max: x.AddDays(1)));
                
                timeRange
                    .Select(range => OxyPlot.Axes.DateTimeAxis.ToDouble(range.min))
                    .BindTo(TimeAxis, x => x.Minimum)
                    .DisposeWith(disposables);
                timeRange.Select(range => OxyPlot.Axes.DateTimeAxis.ToDouble(range.max))
                    .BindTo(TimeAxis, x => x.Maximum)
                    .DisposeWith(disposables);
            });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        /// <summary>
        /// Defines the <see cref="Forecast"/> property.
        /// </summary>
        public static readonly StyledProperty<IReadOnlyList<ForecastData>> ForecastProperty =
            AvaloniaProperty.Register<BasicForecastGraph, IReadOnlyList<ForecastData>>(nameof(Forecast));
        
        /// <summary>
        /// Gets or sets the type of weather for which the image should be displayed
        /// </summary>
        public IReadOnlyList<ForecastData> Forecast
        {
            get => GetValue(ForecastProperty);
            set => SetValue(ForecastProperty, value);
        }
    }
}