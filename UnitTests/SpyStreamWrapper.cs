using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UnitTests
{
    public class SpyStreamWrapper : Stream
    {
        private readonly Stream _base_stream;

        public SpyStreamWrapper(Stream comStream)
        {
            _base_stream = comStream;
        }

        public override void Flush()
        {
            Console.WriteLine("SpyStreamWrapper.Flush called");
            _base_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("SpyStreamWrapper.Read called");
            Console.WriteLine("offset:" + offset + " count:" + count);
            int result = _base_stream.Read(buffer, offset, count);

            Console.Write("buffer:");
            for (int i = 0; i < result && i < 100 && i < buffer.Length; i++)
            {
                Console.Write(buffer[i] + " ");
            }
            Console.Write(Environment.NewLine);

            Console.WriteLine("returns:" + result);
            Console.WriteLine("Bonus - Position:" + _base_stream.Position);
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            Console.WriteLine("SpyStreamWrapper.Seek called");
            Console.WriteLine("offset:" + offset + " origin:" + origin);
            var result = _base_stream.Seek(offset, origin);
            Console.WriteLine("returns:" + result);
            return result;
        }

        public override void SetLength(long value)
        {
            Console.WriteLine("SpyStreamWrapper.SetLength called");
            _base_stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Console.WriteLine("SpyStreamWrapper.Write called");
            _base_stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                Console.WriteLine("SpyStreamWrapper.CanRead get called");
                var result = _base_stream.CanRead;
                Console.WriteLine("returns:" + result);
                return result;
            }
        }
        public override bool CanSeek
        {
            get
            {
                Console.WriteLine("SpyStreamWrapper.CanSeek get called");
                var result = _base_stream.CanSeek;
                Console.WriteLine("returns:" + result);
                return result;
            }
        }

        public override bool CanWrite { get { Console.WriteLine("SpyStreamWrapper.CanWrite get called"); return _base_stream.CanWrite; } }

        public override long Length
        {
            get
            {
                Console.WriteLine("SpyStreamWrapper.Length get called");
                Console.WriteLine("returns:" + _base_stream.Length);
                return _base_stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                Console.WriteLine("SpyStreamWrapper.Position get called");
                Console.WriteLine("returns:" + _base_stream.Position);
                return _base_stream.Position;
            }
            set
            {
                Console.WriteLine("SpyStreamWrapper.Position set called");
                Console.WriteLine("value:" + value);
                _base_stream.Position = value;
            }
        }
    }
}
