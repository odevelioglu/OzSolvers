using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OzAlgo;

namespace AlgoSolverTests
{
    [TestClass]
    public class MatTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            for(int i = 0; i< 15; i++)
                Debug.WriteLine(SumIdCount(i));
            
        }

        public static long SumIdCount(long n)
        {
            return GetSum(1, Fac(n));
        }
        public static long Fac(long a)
        {
            if (a <= 1)
                return 1;
            
            return a * Fac(a - 1);
            
        }
        public static long LowestPow2(long n)
        {
            var log = Math.Log(n, 2);
            var floor = Math.Floor(log);
            return Convert.ToInt64(Math.Pow(2, floor));
        }

        public static bool IsPowOf2(long n)
        {
            return Math.Log(n, 2) % 1 == 0;
        }

        public static long Lg(long n)
        {
            return Convert.ToInt32(Math.Log(n, 2));
        }

        public static long GetSum(int depth, long n)
        {
            if (n == 0)
                return 0;

            var lowestPow2 = LowestPow2(n);

            if (lowestPow2 == n)
                return Lg(n);

            var i = lowestPow2*(Lg(lowestPow2)+depth);
            
            //Debug.WriteLine(i);

            var remaining = n - lowestPow2;

            if (IsPowOf2(remaining))
                return i + remaining * (Lg(remaining)+depth);

            return i + GetSum(depth + 1, remaining);
        }
    }

    public class Number
    {
        #region Fields
        private static long _valueDivision = 1000000000;
        private static int _valueDivisionDigitCount = 9;
        private static string _formatZeros = "000000000";
        List<long> _value;
        #endregion

        #region Properties
        public int ValueCount { get { return _value.Count; } }
        public long ValueAsLong
        {
            get
            {
                return long.Parse(ToString());
            }
            set { SetValue(value.ToString()); }
        }
        #endregion

        #region Constructors
        public Number()
        {
            _value = new List<long>();
        }
        public Number(long value)
            : this()
        {
            SetValue(value.ToString());
        }
        public Number(string value)
            : this()
        {
            SetValue(value);
        }
        private Number(List<long> list)
        {
            _value = list;
        }
        #endregion

        #region Public Methods
        public void SetValue(string value)
        {
            _value.Clear();
            bool finished = false;
            while (!finished)
            {
                if (value.Length > _valueDivisionDigitCount)
                {
                    _value.Add(long.Parse(value.Substring(value.Length - _valueDivisionDigitCount)));
                    value = value.Remove(value.Length - _valueDivisionDigitCount, _valueDivisionDigitCount);
                }
                else
                {
                    _value.Add(long.Parse(value));
                    finished = true;
                }
            }
        }
        #endregion

        #region Static Methods
        public static Number operator +(Number c1, Number c2)
        {
            return Add(c1, c2);
        }
        public static Number operator *(Number c1, Number c2)
        {
            return Mul(c1, c2);
        }
        private static Number Add(Number value1, Number value2)
        {
            Number result = new Number();
            int count = Math.Max(value1._value.Count, value2._value.Count);
            long reminder = 0;
            long firstValue, secondValue;
            for (int i = 0; i < count; i++)
            {
                firstValue = 0;
                secondValue = 0;
                if (value1._value.Count > i)
                {
                    firstValue = value1._value[i];
                }
                if (value2._value.Count > i)
                {
                    secondValue = value2._value[i];
                }
                reminder += firstValue + secondValue;
                result._value.Add(reminder % _valueDivision);
                reminder /= _valueDivision;
            }
            while (reminder > 0)
            {
                result._value.Add(reminder % _valueDivision);
                reminder /= _valueDivision;
            }
            return result;
        }
        private static Number Mul(Number value1, Number value2)
        {
            List<List<long>> values = new List<List<long>>();
            for (int i = 0; i < value2._value.Count; i++)
            {
                values.Add(new List<long>());
                long lastremain = 0;
                for (int j = 0; j < value1._value.Count; j++)
                {
                    values[i].Add(((value1._value[j] * value2._value[i] + lastremain) % _valueDivision));
                    lastremain = ((value1._value[j] * value2._value[i] + lastremain) / _valueDivision);
                    //result.Add(();
                }
                while (lastremain > 0)
                {
                    values[i].Add((lastremain % _valueDivision));
                    lastremain /= _valueDivision;
                }
            }
            List<long> result = new List<long>();
            for (int i = 0; i < values.Count; i++)
            {
                for (int j = 0; j < i; j++)
                {
                    values[i].Insert(0, 0);
                }
            }
            int count = values.Select(list => list.Count).Max();
            int index = 0;
            long lastRemain = 0;
            while (count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (values[i].Count > index)
                        lastRemain += values[i][index];
                }
                result.Add((lastRemain % _valueDivision));
                lastRemain /= _valueDivision;
                count -= 1;
                index += 1;
            }
            while (lastRemain > 0)
            {
                result.Add((lastRemain % _valueDivision));
                lastRemain /= _valueDivision;
            }
            return new Number(result);
        }
        #endregion

        #region Overriden Methods Of Object
        public override string ToString()
        {
            string result = string.Empty;
            for (int i = 0; i < _value.Count; i++)
            {
                result = _value[i].ToString(_formatZeros) + result;
            }
            return result.TrimStart('0');
        }
        #endregion
    }

    class Program
    {
        static void Main(string[] args)
        {
            Number number1 = new Number(5000);
            DateTime dateTime = DateTime.Now;
            string s = Factorial(number1).ToString();
            TimeSpan timeSpan = DateTime.Now - dateTime;
            long sum = s.Select(c => (long)(c - '0')).Sum();
        }
        static Number Factorial(Number value)
        {
            if (value.ValueCount == 1 && value.ValueAsLong == 2)
            {
                return value;
            }
            return Factorial(new Number(value.ValueAsLong - 1)) * value;
        }
    }
}
