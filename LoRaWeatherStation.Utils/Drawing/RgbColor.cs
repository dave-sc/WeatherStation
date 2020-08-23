using System;

namespace LoRaWeatherStation.Utils.Drawing
{
    public readonly struct RgbColor
    {
        public RgbColor(double a, double r, double g, double b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }
        
        public RgbColor(byte a, byte r, byte g, byte b)
            : this(a / 255d, r / 255d, g / 255d, b / 255d)
        {}

        public RgbColor(HsvColor c)
        {
            A = c.A;
            
            if (c.S == 0)
            {
                R = c.V;
                G = c.V;
                B = c.V;
                return;
            }

            var vH = (c.H * 6d) % 6d;
            var vI = (int) vH;

            var v1 = c.V * (1 - c.S);
            var v2 = c.V * (1 - c.S * (vH - vI));
            var v3 = c.V * (1 - c.S * (1 - (vH - vI)));

            switch (vI)
            {
                case 0:
                    R = c.V;
                    G = v3;
                    B = v1;
                    break;
                case 1:
                    R = v2;
                    G = c.V;
                    B = v1;
                    break;
                case 2:
                    R = v1;
                    G = c.V;
                    B = v3;
                    break;
                case 3:
                    R = v1;
                    G = v2;
                    B = c.V;
                    break;
                case 4:
                    R = v3;
                    G = v1;
                    B = c.V;
                    break;
                case 5:
                    R = c.V;
                    G = v1;
                    B = v2;
                    break;
                default:
                    throw new ArgumentException(null, nameof(c));
            }
        }

        public double A { get; }
        public double R { get; }
        public double G { get; }
        public double B { get; }

        public HsvColor ToHsv() => new HsvColor(this);
        
        public RgbColor Mix(RgbColor other, double amount) => new RgbColor(A + (other.A - A) * amount, R + (other.R - R) * amount, G + (other.G - G) * amount, B + (other.B - B) * amount);

        public bool Equals(RgbColor other) => A.Equals(other.A) && R.Equals(other.R) && G.Equals(other.G) && B.Equals(other.B);

        public override bool Equals(object obj) => obj is RgbColor other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(A, R, G, B);
    }
}