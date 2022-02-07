using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad.SignalTypes
{
    public class TriangleSignal : Signal
    {
        public TriangleSignal(double A, double f, double fi)
            : base(A, f, fi)
        {
        }
        public override double GetValue(int x, int N)
        {

            double deg = Fi * Math.PI / 180;
            return A * 2 * Math.Asin(Math.Sin(2 * Math.PI * F * x / N + deg)) / Math.PI;
        }
    }
}
