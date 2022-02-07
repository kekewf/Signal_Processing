using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad
{
    public abstract class Signal
    {
        public double Amplite { get; protected set; }
        public double F { get; set; }
        public double Fi { get; set; }

        public Signal(double A, double f, double fi)
        {
            this.Amplite = A;
            this.F = f;
            this.Fi = fi;
        }

        public abstract double GetValue(int x, int N);
    }
    }
}
