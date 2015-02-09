using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSimulator
{
    public class MyTuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public MyTuple(T1 i1, T2 i2)
        {
            this.Item1 = i1;
            this.Item2 = i2;
        }

        public override string ToString()
        {
            return Item1.ToString() + "," + Item2.ToString();
        }

    }

    public static class ListExtensions
    {
        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }

        public static List<MyTuple<double, double>> CDF(this List<double> values, int nBins)
        {
            List<MyTuple<double, double>> retVal = new List<MyTuple<double, double>>();
            int count = values.Count;


            double startingpt = values.Min();
            double binsize = (values.Max() - startingpt) / nBins;
            for (int i = 1; i <= startingpt / binsize; i++)
                retVal.Add(new MyTuple<double, double>(i * binsize, 0));

            for (int i = 0; i <= nBins; i++)
            {
                retVal.Add(new MyTuple<double, double>(startingpt + i * binsize, 0));
            }

            foreach (double val in values)
            {
                foreach (MyTuple<double, double> t in retVal)
                {
                    if (val <= t.Item1)
                        t.Item2 = t.Item2 + 1.00;

                }
            }

            foreach (MyTuple<double, double> t in retVal)
            {
                t.Item2 = t.Item2 / count;
            }

            return retVal;
        }


        public static List<MyTuple<MyTuple<double,double>,double>> PDF(this List<double> values, double binsize)
        {
            List<MyTuple<MyTuple<double, double>, double>> retVal = new List<MyTuple<MyTuple<double, double>, double>>();
            int count = values.Count;

            double endingpt = values.Max();
            double i = 0;
            while(i<=endingpt)
            {
                retVal.Add(new MyTuple<MyTuple<double, double>, double>(new MyTuple<double, double>(i, i+binsize), 0.0));
                i = i + binsize;
            }

            foreach (double val in values)
            {
                foreach (MyTuple<MyTuple<double, double>, double> t in retVal)
                {
                    if (val >= t.Item1.Item1 && val < t.Item1.Item2)
                        t.Item2 = t.Item2 + 1.00;
                }
            }

            foreach (MyTuple<MyTuple<double, double>, double> t in retVal)
            {
                t.Item2 = t.Item2 / values.Count;
            }

            return retVal;
        }


        public static void WriteToFile<T>(this List<T> values, string filePath)
        {
            try
            {

                if (!File.Exists(filePath))
                {
                    using (StreamWriter sw = File.CreateText(filePath))
                    {
                    }
                }

                using (StreamWriter sw = File.AppendText(filePath))
                {
                    foreach (T t in values)
                    {
                        sw.WriteLine(t.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Log.exception("writing list to file: " + e);
            }
        }


        private static int Partition<T>(this IList<T> list, int start, int end, Random rnd = null) where T : IComparable<T>
        {
            if (rnd != null)
                list.Swap(end, rnd.Next(start, end));

            var pivot = list[end];
            var lastLow = start - 1;
            for (var i = start; i < end; i++)
            {
                if (list[i].CompareTo(pivot) <= 0)
                    list.Swap(i, ++lastLow);
            }
            list.Swap(end, ++lastLow);
            return lastLow;
        }

        /// <summary>
        /// Returns Nth smallest element from the list. Here n starts from 0 so that n=0 returns minimum, n=1 returns 2nd smallest element etc.
        /// Note: specified list would be mutated in the process.
        /// Reference: Introduction to Algorithms 3rd Edition, Corman et al, pp 216
        /// </summary>
        public static T NthOrderStatistic<T>(this IList<T> list, int n, Random rnd = null) where T : IComparable<T>
        {
            return NthOrderStatistic(list, n, 0, list.Count - 1, rnd);
        }
        private static T NthOrderStatistic<T>(this IList<T> list, int n, int start, int end, Random rnd) where T : IComparable<T>
        {
            while (true)
            {
                var pivotIndex = list.Partition(start, end, rnd);
                if (pivotIndex == n)
                    return list[pivotIndex];

                if (n < pivotIndex)
                    end = pivotIndex - 1;
                else
                    start = pivotIndex + 1;
            }
        }

        public static void Swap<T>(this IList<T> list, int i, int j)
        {
            if (i == j)   //This check is not required but Partition function may make many calls so its for perf reason
                return;
            var temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }

        /// <summary>
        /// Note: specified list would be mutated in the process.
        /// </summary>
        public static T Median<T>(this IList<T> list) where T : IComparable<T>
        {
            return list.NthOrderStatistic((list.Count - 1) / 2);
        }

        public static double Median<T>(this IEnumerable<T> sequence, Func<T, double> getValue)
        {
            var list = sequence.Select(getValue).ToList();
            var mid = (list.Count - 1) / 2;
            return list.NthOrderStatistic(mid);
        }
    
    
    }



}
