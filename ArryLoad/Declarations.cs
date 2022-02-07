using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArryLoad
{//abstract
    public  class Declarations
    {
        private bool IsCorrectDouble(string str, out double value)
        {
            bool result = true;

            if (double.TryParse(str, out value))
            {
                if (value >= 0)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}
