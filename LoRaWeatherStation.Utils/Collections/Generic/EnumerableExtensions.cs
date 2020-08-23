using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LoRaWeatherStation.Utils.Collections.Generic
{
    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException();

            return new ObservableCollection<T>(enumerable);
        }
    }
}