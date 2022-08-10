using System;
using System.Collections.Generic;

namespace LoRaWeatherStation.Utils.Collections.Generic
{
    public class RelayEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public RelayEqualityComparer(Func<T, T, bool> equals)
            : this(equals, _ => 0)
        {
        }

        public RelayEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }
        
        public static RelayEqualityComparer<T> Create(Func<T, T, bool> equals) => new RelayEqualityComparer<T>(equals);
        
        public static RelayEqualityComparer<T> Create(Func<T, T, bool> equals, Func<T, int> getHashCode) => new RelayEqualityComparer<T>(equals, getHashCode);

        public bool Equals(T x, T y)
        {
            return _equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}