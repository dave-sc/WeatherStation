using System;

namespace LoRaWeatherStation.Utils.Drawing
{
    public readonly struct HsvColor
    {
        public HsvColor(double a, double h, double s, double v)
        {
            A = a;
            H = h == 1.0 ? 0 : h;
            S = s;
            V = v;
        }
        
        public HsvColor(byte a, byte h, byte s, byte v)
            : this(a / 255d, h / 255d, s / 255d, v / 255d)
        {}

        public HsvColor(RgbColor c)
        {
            A = c.A;
            
            var max = Math.Max(c.R, Math.Max(c.G, c.B));
            var min = Math.Min(c.R, Math.Min(c.G, c.B));
            var d = max - min;

            H = 0d;
            S = 0d;
            V = max;
            if (d != 0)
            {
                S = d / max;

                var dR = (((max - c.R) / 6d) + (d / 2d)) / d;
                var dG = (((max - c.G) / 6d) + (d / 2d)) / d;
                var dB = (((max - c.B) / 6d) + (d / 2d)) / d;
                
                if (max == c.R) H = dB - dG;
                if (max == c.G) H = (1d / 3d) + dR - dB;
                if (max == c.B) H = (2d / 3d) + dG - dR;
                if (H < 0) H += 1;
                if (H >= 1) H -= 1;
            }
        }
        
        public double A { get; }
        public double H { get; }
        public double S { get; }
        public double V { get; }

        public RgbColor ToRgb() => new RgbColor(this);
        
        public HsvColor Mix(HsvColor other, double amount) => new HsvColor(A + (other.A - A) * amount, MixHue(H, other.H, amount), S + (other.S - S) * amount, V + (other.V - V) * amount);

        private static double MixHue(double thisHue, double otherHue, double amount)
        {
            if (otherHue < thisHue)
            {
                var t = thisHue;
                thisHue = otherHue;
                otherHue = t;
                amount = 1.0 - amount;
            }
            if (otherHue - thisHue > 0.5) 
                thisHue += 1.0;
            
            var newHue = thisHue + (otherHue - thisHue) * amount;
            if (newHue > 1.0)
                newHue -= 1.0;
            
            return newHue;
        }

        public bool Equals(HsvColor other) => A.Equals(other.A) && H.Equals(other.H) && S.Equals(other.S) && V.Equals(other.V);

        public override bool Equals(object obj) => obj is HsvColor other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(A, H, S, V);
    }
}