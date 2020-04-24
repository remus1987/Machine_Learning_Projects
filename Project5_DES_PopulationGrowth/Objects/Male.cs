using System;
using System.Collections.Generic;
using System.Text;

namespace Project5_DES_PopulationGrowth.Objects
{
    public class Male : Individual
    {
        public Male(int age) : base(age)
        {
        }
        public override string ToString()
        {
            return base.ToString() + " Male";
        }
    }
}
