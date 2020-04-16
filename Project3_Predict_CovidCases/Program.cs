using System;
using System.Collections;

namespace Project3_Predict_CovidCases
{
    class Program
    {
        static void Main(string[] args)
        {
            // express the date in DDMM format
            var date = new double[]
            {
                //2301, 2401, 2501, 2601, 2701, 2801, 2901, 3001, 3101,
                //0102, 0202, 0302, 0402, 0502, 0602, 0702, 0802, 0902,
                //1002, 1102, 1202, 1303, 1402, 1502, 1602, 1702, 1802,
                //1902, 2002, 2102, 2202, 2302, 2402, 2502, 2602, 2702,
                //2802, 2902, 0103, 0203, 0303, 0403, 0503, 0603, 0703,
                //0803, 0903, 1003, 1103, 1203, 1303, 1403, 1503, 1603,
                //1703, 1803, 1903
                1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 
                17, 18, 19, 20, 21, 22
            };
            var dailyCases = new double[]
            {
                //265, 472, 698, 785, 1781, 1477, 1755, 2010, 2127,
                //2603, 2838, 3239, 3915, 3721, 3173, 3437, 2676,
                //3001, 2546, 2035, 14153, 5151, 2662, 2097, 2132,
                //2003, 1852, 516, 977, 996, 978, 554, 882, 741, 992,
                1292, 1503, 1989, 1981, 1858, 2573, 2298, 3111, 3625,
                4049, 3892, 4390, 4567, 7266, 8362, 10907, 11170,
                12940, 12897, 15748, 20584, 26111
            };

            double rSquared, intercept, slope;
            LinearRegression(date, dailyCases, out rSquared, out intercept, out slope);

            Console.WriteLine($"R-squared = {rSquared}");
            Console.WriteLine($"Intercept = {intercept}");
            Console.WriteLine($"Slope = {slope}");

            var predictedValue = (slope * 29) + intercept;
            Console.WriteLine($"Prediction for 29/March/2020: {predictedValue}");
        }

        //Fits a line to a collection of (date, dailyCases) points
        public static void LinearRegression(double[] dateVals, double[] dailyCasesVals,
            out double rSquared, out double yIntercept, out double slope)
        {
            if (dateVals.Length != dailyCasesVals.Length)
            {
                throw new Exception("Input values should be with the same length.");
            }

            double sumOfX = 0;
            double sumOfY = 0;
            double sumOfXSq = 0;
            double sumOfYSq = 0;
            double sumCodeviates = 0;

            for (int i = 0; i < dateVals.Length; i++)
            {
                var x = dateVals[i];
                var y = dailyCasesVals[i];
                sumOfX += x;
                sumOfY += y;
                sumOfXSq += x * x;
                sumOfYSq += y * y;
            }

            var count = dateVals.Length;
            var ssX = sumOfXSq - ((sumOfX * sumOfY) / count);
            var ssY = sumOfYSq - ((sumOfY * sumOfY) / count);

            var rNumerator = (count * sumCodeviates) - (sumOfX * sumOfY);
            var rDenom = (count * sumOfXSq - (sumOfX * sumOfX)) * (count * sumOfYSq - (sumOfY * sumOfY));
            var sCo = sumCodeviates - ((sumOfX * sumOfY) / count);

            var meanX = sumOfX / count;
            var meanY = sumOfY / count;
            var dblR = rNumerator / Math.Sqrt(rDenom);

            rSquared = dblR * dblR;
            yIntercept = meanY - ((sCo / ssX) * meanX);
            slope = sCo / ssX;
        }
    }
}
