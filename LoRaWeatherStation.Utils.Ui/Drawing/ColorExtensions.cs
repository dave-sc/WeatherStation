using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Media;

namespace LoRaWeatherStation.Utils.Drawing
{
    public static class ColorExtensions
    {
        public static RgbColor ToRgb(this Color c) => new RgbColor(c.A, c.R, c.G, c.B);
        
        public static HsvColor ToHsv(this Color c) => new HsvColor(c.ToRgb());
        
        public static Color ToColor(this RgbColor c) => new Color((byte)(c.A * 255), (byte)(c.R * 255), (byte)(c.G * 255), (byte)(c.B * 255));
    }

    public static class GradientStopExtensions
    {
        public static Color GetColorAtOffset(this IReadOnlyList<GradientStop> stops, double offset)
        {
            if (stops == null)
                throw new ArgumentNullException(nameof(stops));
            if (stops.Count == 0)
                throw new ArgumentException("Cannot get color at offset in empty gradient stop collection", nameof(stops));
            
            var orderedStops = stops.OrderBy(x => x.Offset).ToArray();

            var firstStop = orderedStops.First();
            if (offset <= firstStop.Offset)
                return firstStop.Color;
            
            var lastStop = orderedStops.Last();
            if (offset >= lastStop.Offset)
                return lastStop.Color;
                
            if (orderedStops.FirstOrDefault(x => Math.Abs(x.Offset - offset) < 0.001) is { } matchingStop)
                return matchingStop.Color;

            var from = orderedStops.Last(x => x.Offset <= offset);
            var to = orderedStops.First(x => x.Offset >= offset);
            var delta = to.Offset - from.Offset;
            var amount = (offset-from.Offset) / delta;
            return from.Color.ToHsv().Mix(to.Color.ToHsv(), amount).ToRgb().ToColor();
        }
    }
}