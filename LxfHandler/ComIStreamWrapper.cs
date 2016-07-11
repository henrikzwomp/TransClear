using System;

using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace LxfHandler
{
    public class ComIStreamWrapper : Stream
    {
        private IStream _istream_object;
        private IntPtr _buffer_pointer;

        public ComIStreamWrapper(IStream comStream)
        {
            _istream_object = comStream;
            _buffer_pointer = Marshal.AllocCoTaskMem(8);
            Marshal.WriteInt64(_buffer_pointer, 0);
        }

        ~ComIStreamWrapper()
        {
            ReleaseStream();
        }

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes 
        /// any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _istream_object.Commit(0);
        }

        /// <summary>
        /// When overridden in a derived class, reads a sequence of bytes from the current stream and advances the position within the stream 
        /// by the number of bytes read.
        /// </summary>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between <paramref name="offset" /> and (<paramref name="offset" /> + <paramref name="count" /> - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in <paramref name="buffer" /> at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var position = Marshal.ReadInt64(_buffer_pointer);

            if (position + count > this.Length)
                throw new ArgumentException("Current position and count together is "
                    + "greater than length of stream. "
                    + "Position: " + position + " "
                    + "Offset: " + offset + " "
                    + "Count: " + count + " "
                    + "Length: " + Length + " ");

            if (offset + count > buffer.Length)
                throw new ArgumentException("Offset and count together is "
                    + "greater than length of buffer. "
                    + "Position: " + position + " "
                    + "Offset: " + offset + " "
                    + "Count: " + count + " "
                    + "Length: " + Length + " ");

            var pointer_to_number_of_bytes_read = Marshal.AllocCoTaskMem(8);
            Marshal.WriteInt64(pointer_to_number_of_bytes_read, 0);
           
            //  Read into the buffer and advance the position.
            if (offset != 0)
            {
                var temp_buffer = new byte[count];

                _istream_object.Read(temp_buffer, count, pointer_to_number_of_bytes_read);

                Array.Copy(temp_buffer, 0, buffer, offset, temp_buffer.Length);
            }
            else
            {
                _istream_object.Read(buffer, count, pointer_to_number_of_bytes_read);
            }

            var position_moved = Marshal.ReadInt64(pointer_to_number_of_bytes_read);

            Marshal.FreeCoTaskMem(pointer_to_number_of_bytes_read);
            
            position += position_moved;
            Marshal.WriteInt64(_buffer_pointer, position);

            return (int) position_moved;
        }

        /// <summary>
        /// When overridden in a derived class, sets the position within the current stream.
        /// </summary>
        /// <param name="offset">A byte offset relative to the <paramref name="origin" /> parameter.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin" /> indicating the reference point used to obtain the new position.</param>
        /// <returns>
        /// The new position within the current stream.
        /// </returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            _istream_object.Seek(offset, (int)origin, _buffer_pointer);
            return Marshal.ReadInt64(_buffer_pointer);
        }

        public override void SetLength(long value)
        {
            _istream_object.SetSize(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead { get { return true; } }
        public override bool CanSeek { get { return true; } }
        public override bool CanWrite { get { return false; } }

        private long _length = -1;
        public override long Length
        {
            get
            {
                if (_length == -1)
                {
                    //  Get the statistics of the COM stream, return the size.
                    STATSTG stat;
                    _istream_object.Stat(out stat, 1);
                    _length = stat.cbSize;
                }

                return _length;
            }
        }

        public override long Position
        {
            get { return Marshal.ReadInt64(_buffer_pointer); }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public void ReleaseStream()
        {
            Marshal.FreeCoTaskMem(_buffer_pointer);

            if (_istream_object != null && Marshal.IsComObject(_istream_object)) Marshal.ReleaseComObject(_istream_object); // Must check that object is ComObject so code won't break when tests are executed.
            _istream_object = null;
        }
    }
}
