using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LoRaWeatherStation.DataModel;
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
                    .Select(x => x.OrderBy(y => y.Time).ToArray())
                    .Select(x => x.Where(y => y.Time <= (x.FirstOrDefault()?.Time.Date ?? DateTime.MinValue.Date).AddDays(1)).ToArray());

                forecast.Select(x => x.Select(y => new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(y.Time), y.Temperature)))
                    .BindTo(TempSeries, x => x.Items)
                    .DisposeWith(disposables);
                forecast.Select(x => x.Max(y => y.Temperature))
                    .Select(x => Math.Ceiling(x / 5) * 5)
                    .BindTo(TempAxis, x => x.Maximum)
                    .DisposeWith(disposables);
                forecast.Select(x => x.Min(y => y.Temperature))
                    .Select(x => Math.Floor(x / 5) * 5)
                    .BindTo(TempAxis, x => x.Minimum)
                    .DisposeWith(disposables);
                
                forecast.Select(x => x.Select(y => new DataPoint(OxyPlot.Axes.DateTimeAxis.ToDouble(y.Time), y.Precipitation)))
                    .BindTo(RainSeries, x => x.Items)
                    .DisposeWith(disposables);
                
                var timeRange = forecast.Select(x => x.FirstOrDefault()?.Time.Date ?? DateTime.MinValue.Date)
                    .Select(x => (min: x, max: x.AddDays(1)));
                timeRange.Select(x => x.min)
                    .BindTo(TimeAxis, x => x.FirstDateTime)
                    .DisposeWith(disposables);
                timeRange.Select(x => x.max)
                    .BindTo(TimeAxis, x => x.LastDateTime)
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