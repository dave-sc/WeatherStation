using System;
using System.Collections.Generic;

namespace LoRaWeatherStation.Utils.Data.Encoding
{
    /// <summary>
    /// Provides methods to encode or decode arbitrary byte sequences using the Consistent Overhead Byte Stuffing (COBS) algorithm
    /// </summary>
    /// <remarks>
    /// C-Version originally written by Christopher Baker, Copyright (c) 2011 and released under MIT-License
    /// See: https://github.com/bakercp/PacketSerial/blob/8c11d3aca3513e1f06e4538142bd82096d584cf9/src/Encoding/COBS.h
    /// </remarks>
    public class Cobs
    {
        public static byte[] Encode(IReadOnlyList<byte> data)
        {
            var result = new byte[data.Count + data.Count / 254 + 1];
            
            var readIndex  = 0;
            var writeIndex = 1;
            var lastZeroIndex  = 0;
            byte offsetToNextZero = 1;

            while (readIndex < data.Count)
            {
                if (data[readIndex] == 0)
                {
                    result[lastZeroIndex] = offsetToNextZero;
                    offsetToNextZero = 1;
                    lastZeroIndex = writeIndex++;
                    
                    readIndex++;
                }
                else
                {
                    result[writeIndex++] = data[readIndex++];
                    offsetToNextZero++;

                    if (offsetToNextZero == 0xFF)
                    {
                        result[lastZeroIndex] = offsetToNextZero;
                        offsetToNextZero = 1;
                        lastZeroIndex = writeIndex++;
                    }
                }
            }

            result[lastZeroIndex] = offsetToNextZero;
            
            Array.Resize(ref result, writeIndex);

            return result;
        }
        
        public static byte[] Decode(IReadOnlyList<byte> data)
        {
            if (data.Count == 0)
                return Array.Empty<byte>();

            var result = new byte[data.Count];
            
            var readIndex  = 0;
            var writeIndex = 0;

            while (readIndex < data.Count)
            {
                var offsetToNextZero = data[readIndex];

                if (readIndex + offsetToNextZero > data.Count && offsetToNextZero != 1)
                    throw new ArgumentException($"Provided COBS data contains invalid offset to next zero byte at index {readIndex}");

                readIndex++;

                for (var i = 1; i < offsetToNextZero; i++) 
                    result[writeIndex++] = data[readIndex++];

                if (offsetToNextZero != 0xFF && readIndex != data.Count) 
                    result[writeIndex++] = 0;
            }
            
            Array.Resize(ref result, writeIndex);

            return result;
        }
    }
}