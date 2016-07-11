using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DevTools
{
    public class LoggingStreamWrapper : Stream
    {
        private readonly Stream _base_stream;
        private readonly FileLogger _logger;

        public LoggingStreamWrapper(Stream comStream, FileLogger logger)
        {
            _base_stream = comStream;
            _logger = logger;
        }

        public override void Flush()
        {
            _logger.LogMessage("LoggingStreamWrapper.Flush called");
            _base_stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _logger.LogMessage("LoggingStreamWrapper.Read called");
            _logger.LogMessage("offset:" + offset + " count:" + count);
            int result = _base_stream.Read(buffer, offset, count);

            string buffer_string = "";
            for (int i = 0; i < result && i < 100 && i < buffer.Length; i++)
            {
                buffer_string += buffer[i] + " ";
            }

            _logger.LogMessage("buffer:" + buffer_string);

            _logger.LogMessage("returns:" + result);

            _logger.LogMessage("Bonus - Position:" + _base_stream.Position);
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            _logger.LogMessage("LoggingStreamWrapper.Seek called");
            _logger.LogMessage("offset:" + offset + " origin:" + origin);
            var result = _base_stream.Seek(offset, origin);
            _logger.LogMessage("returns:" + result);
            _logger.LogMessage("Bonus - Position:" + _base_stream.Position);
            return result;
        }

        public override void SetLength(long value)
        {
            _logger.LogMessage("LoggingStreamWrapper.SetLength called");
            _base_stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _logger.LogMessage("LoggingStreamWrapper.Write called");
            _base_stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                _logger.LogMessage("LoggingStreamWrapper.CanRead get called");
                var result = _base_stream.CanRead;
                _logger.LogMessage("returns:" + result);
                return result;
            }
        }
        public override bool CanSeek
        {
            get
            {
                _logger.LogMessage("LoggingStreamWrapper.CanSeek get called");
                var result = _base_stream.CanSeek;
                _logger.LogMessage("returns:" + result);
                return result;
            }
        }

        public override bool CanWrite { get { _logger.LogMessage("LoggingStreamWrapper.CanWrite get called"); return _base_stream.CanWrite; } }

        public override long Length
        {
            get
            {
                _logger.LogMessage("LoggingStreamWrapper.Length get called");
                _logger.LogMessage("returns:" + _base_stream.Length);
                return _base_stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                _logger.LogMessage("LoggingStreamWrapper.Position get called");
                _logger.LogMessage("returns:" + _base_stream.Position);
                return _base_stream.Position;
            }
            set
            {
                _logger.LogMessage("LoggingStreamWrapper.Position set called");
                _logger.LogMessage("value:" + value);
                _base_stream.Position = value;
            }
        }
    }
}
