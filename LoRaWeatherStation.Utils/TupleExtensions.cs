using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

namespace LoRaWeatherStation.Utils
{
    public static class TupleExtensions
    {
        public static IEnumerable AsEnumerable(this ITuple tuple)
        {
            for (int i = 0; i < tuple.Length; i++)
                yield return tuple[i];
        }
    }
}