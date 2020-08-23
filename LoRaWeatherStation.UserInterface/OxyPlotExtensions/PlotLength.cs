using System;
using System.Globalization;
using OxyPlot;

namespace LoRaWeatherStation.UserInterface.OxyPlotExtensions
{
    /// <summary>
    /// Represents absolute or relative lengths in data or screen space.
    /// </summary>
    public struct PlotLength : IEquatable<PlotLength>
    {
        public PlotLength(double value, PlotLengthUnit unit)
        {
            Value = value;
            Unit = unit;
        }

        /// <summary>Gets the value.</summary>
        /// <value>The value.</value>
        public double Value { get; }

        /// <summary>Gets the type of the unit.</summary>
        /// <value>The type of the unit.</value>
        public PlotLengthUnit Unit { get; }

        public static bool operator ==(PlotLength a, PlotLength b) => a.Value.Equals(b.Value) && a.Unit == b.Unit;
        public static bool operator !=(PlotLength a, PlotLength b) => !(a == b);
        
        public bool Equals(PlotLength other) => this == other;
        public override bool Equals(object obj) => obj is PlotLength other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(Value, (int) Unit);

        /// <summary>
        /// Gets a string representation of the <see cref="PlotLength"/>.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            var unit = Unit switch
            {
                PlotLengthUnit.Data => "data",
                PlotLengthUnit.ScreenUnits => "screen",
                PlotLengthUnit.RelativeToViewport => "viewport",
                PlotLengthUnit.RelativeToPlotArea => "plot",
                _ => throw new InvalidOperationException()
            };
            return $"{Value},{unit}";
        }

        /// <summary>
        /// Parses a string to return a <see cref="PlotLength"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns>The <see cref="PlotLength"/>.</returns>
        public static PlotLength Parse(string s)
        {
            s = s.ToLowerInvariant();
            var splinters = s.Split(',');
            if (splinters.Length != 2)
                throw new ArgumentException();
            
            var value = double.Parse(splinters[0], CultureInfo.InvariantCulture);
            var unit = splinters[1] switch
            {
                "data" => PlotLengthUnit.Data,
                "screen" => PlotLengthUnit.ScreenUnits,
                "viewport" => PlotLengthUnit.RelativeToViewport,
                "plot" => PlotLengthUnit.RelativeToPlotArea,
                _ => throw new ArgumentException()
            };
            
            return new PlotLength(value, unit);
        }
    }
}