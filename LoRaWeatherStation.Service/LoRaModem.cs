using System;
using System.Buffers;
using System.IO.Pipelines;
using System.IO.Ports;
using System.Threading.Tasks;
using LoRaWeatherStation.Utils.Data.Encoding;

namespace LoRaWeatherStation.Service
{
    public class LoRaDataReceivedEventArgs
    {
        public LoRaDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }

        public byte[] Data { get; }
    }
    
    public class LoRaModem : IDisposable
    {
        private readonly string _serialPort;

        private SerialPort _serial;
        private Pipe _pipe;
        private bool _isDiposed;

        public event EventHandler<LoRaDataReceivedEventArgs> DataReceived;
        
        public LoRaModem(string serialPort)
        {
            _serialPort = serialPort;
        }
        
        public void Open()
        {
            if (_isDiposed)
                throw new ObjectDisposedException("");
            
            if (IsOpen)
                return;

            Close();
            _pipe = new Pipe();
            _serial = new SerialPort(_serialPort, 9600, Parity.None, 8, StopBits.One);
            _serial.DataReceived += OnPacketIncoming;
            _serial.ReceivedBytesThreshold = 1;
            _serial.Open();
            _serial.DtrEnable = true;
        }

        public bool IsOpen
        {
            get
            {
                if (_isDiposed)
                    throw new ObjectDisposedException("");
                
                return _pipe != null && _serial?.IsOpen == true;
            }
        }

        public void Close()
        {
            if (_isDiposed)
                throw new ObjectDisposedException("");
            
            var serial = _serial;
            var pipe = _pipe;

            if (serial != null)
            {
                serial.DataReceived -= OnPacketIncoming;
                serial.Close();
                serial.Dispose();
            }

            _serial = null;

            if (pipe != null)
            {
                pipe.Writer.Complete();
                pipe.Reader.Complete();
            }

            _pipe = null;
        }

        private void OnPacketIncoming(object? sender, SerialDataReceivedEventArgs dataReceivedEventArgs)
        {
            if (_isDiposed)
                return;
            
            var serial = _serial;
            var pipe = _pipe;
            if (serial == null || pipe == null)
                return;
            
            var availableBytes = serial.BytesToRead;
            if (availableBytes > 0)
            {
                var buffer = pipe.Writer.GetSpan(availableBytes);
                var bytesRead = serial.BaseStream.Read(buffer);
                pipe.Writer.Advance(bytesRead);
                
                pipe.Writer.FlushAsync().GetAwaiter().GetResult();
            }

            if (pipe.Reader.TryRead(out var readResult))
            {
                var buffer = readResult.Buffer;
                SequencePosition? endPosition;
                
                while ((endPosition = buffer.PositionOf((byte) 0)) != null)
                {
                    var receivedPacket = buffer.Slice(0, endPosition.Value).ToArray();
                    DataReceived?.Invoke(this, new LoRaDataReceivedEventArgs(Cobs.Decode(receivedPacket)));
                    buffer = buffer.Slice(buffer.GetPosition(1, endPosition.Value));
                }
                
                pipe.Reader.AdvanceTo(buffer.Start, buffer.End);
            }
        }

        public Task SendAsync(byte[] data)
        {
            if (_isDiposed)
                throw new ObjectDisposedException("");
            
            var packetToSend = Cobs.Encode(data);
            return _serial.BaseStream.WriteAsync(packetToSend, 0, packetToSend.Length);
        }

        public void Dispose()
        {
            if (_isDiposed)
                return;
            
            Close();
            _isDiposed = true;
        }
    }
}