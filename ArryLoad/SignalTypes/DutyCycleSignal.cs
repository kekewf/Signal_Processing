using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad.SignalTypes
{
    class DutyCycleSignal : Signal
    {
        public double DutyFactor { get; protected set; }
        public DutyCycleSignal(double A, double f, double fi, double dutyFactor)
            : base(A, f, fi)
        {
            this.DutyFactor = dutyFactor;
        }

        public override double GetValue(int x, int N)
        {

            double deg = Fi * Math.PI / 180;
            int coef = (Math.Sin(2 * Math.PI * F * x/N + deg) + 1 >= 2 - 2 * DutyFactor) ? 1 : 0;
            return A * coef;
        }

        public override double GetValue(int x)
        {
            int N = SampleRate;
            double deg = Fi * Math.PI / 180;
            int coef = (Math.Sin(2 * Math.PI * F * x / N + deg) + 1 >= 2 - 2 * DutyFactor) ? 1 : 0;
            return A * coef; ;
        }
    }
}
