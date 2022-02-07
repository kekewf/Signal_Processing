using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad.SignalTypes
{
    public class SinusoidSignal : Signal
    {
        //public SinusoidSignal(double A, double f, double fi)
        //    : base(A, f, fi)
        //{
        //}
        //public override double GetValue(int x, int N)
        //{
        //    double deg = Fi * Math.PI / 180;
        //    return A * Math.Sin(2 * Math.PI * F * x / N + deg);
        //}
        //public override double GetSinValue(int x)
        //{
        //    return Amplitude  * (Math.Sin(2 * Math.PI * Frequency  * x / SampleRate  + (PhaseDeg  * Math.PI / 180)) + SignalDC);
        //}
        public SinusoidSignal(double Amplitude, double Frequency, double PhaseDeg, double SignalDC, int SampleRate)
        : base(Amplitude, Frequency, PhaseDeg, SignalDC, SampleRate)
        { }


        public override double GetValue(int x)
        {

            double PI = 3.141592653589793238462643383279502884197169399;
            //PI = 3.1415926535897932384626433832795028841971693993751058209749445923078;
            return Amplitude * (Math.Sin(2 * PI * Frequency * x / SampleRate + (PhaseDeg * PI / 180)) + SignalDC);
        }

        public override double GetValue(int x, int N)
        {

            double PI = 3.141592653589793238462643383279502884197169399;
            //PI = 3.1415926535897932384626433832795028841971693993751058209749445923078;
            return Amplitude * (Math.Sin(2 * PI * Frequency * x / N + (PhaseDeg * PI / 180)) + SignalDC);
        }
    }

    //public double CosData(double Ampliter, double Freq, int x, int SampleRate, double PhaseDeg, double DC)
    //{
    //    return Ampliter * (Math.Cos(2 * Math.PI * Freq * x / SampleRate + (PhaseDeg * Math.PI / 180)) + DC);
    //}

}
