using System;
using System.Collections.Generic;

namespace ForceDirectedDiagram.Scripts.Helpers
{
    public abstract class SymmetricTupleComparerBase<T> : IEqualityComparer<Tuple<T, T>>
    {
        protected readonly IEqualityComparer<T> comparer = EqualityComparer<T>.Default;

        public bool Equals(Tuple<T, T> x, Tuple<T, T> y)
        {
            if (object.ReferenceEquals(x, y))
                return true;
            else if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
                return false;

            if (comparer.Equals(x.Item1, y.Item1) && comparer.Equals(x.Item2, y.Item2))
                return true;
            if (comparer.Equals(x.Item1, y.Item2) && comparer.Equals(x.Item2, y.Item1))
                return true;
            return false;
        }

        public abstract int GetHashCode(Tuple<T, T> obj);
    }

    public class SymmetricTupleComparer<T> : SymmetricTupleComparerBase<T>
    {
        public override int GetHashCode(Tuple<T, T> obj)
        {
            if (obj == null)
                return 0;
            return HashHelper.SymmetricCombineHash(comparer.GetHashCode(obj.Item1), comparer.GetHashCode(obj.Item2));
        }
    }

    public class SymmetricTupleComparerComplex<T> : SymmetricTupleComparerBase<T>
    {
        public override int GetHashCode(Tuple<T, T> obj)
        {
            if (obj == null)
                return 0;
            return HashHelper.SymmetricCombineHashComplex(comparer.GetHashCode(obj.Item1), comparer.GetHashCode(obj.Item2));
        }
    }

    public static partial class HashHelper
    {
        public static int SymmetricCombineHash(int code1, int code2)
        {
            // in case Item1 and Item2 are identical, code1 == code2 so code1 ^ code2 will always be zero.
            if (code1 == code2)
            {
                // As implemented in practice, hash codes seem to be biased towards small numbers, 
                // so reverse the bytes of the single-item hash to bias it towards a larger number.
                return ReverseBytes(code1);
            }
            // Note that the XOR operator is symmetric
            return code1 ^ code2;
        }

        public static int SymmetricCombineHashComplex(int code1, int code2)
        {
            // This can be useful when input hash codes are biased towards small integers by
            // expanding the input values to larger numbers.

            if (code1 == code2)
            {
                // in case Item1 and Item2 are identical, code1 == code2 so code1 ^ code2 will always be zero.
                return ReverseBytes(code1);
            }
            // Note that the multiplation operator is symmetric
            return unchecked(((ulong)~(uint)code1) * ((ulong)~(uint)code2)).GetHashCode();
        }

        public static Int32 ReverseBytes(Int32 value)
        {
            return unchecked((Int32)ReverseBytes((UInt32)value));
        }

        public static UInt32 ReverseBytes(UInt32 value)
        {
            // https://stackoverflow.com/questions/18145667/how-can-i-reverse-the-byte-order-of-an-int
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
    }
}