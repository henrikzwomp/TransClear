using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices.ComTypes; // IStream
using System.IO;
using System.Runtime.InteropServices;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace UnitTests
{
    public class IStreamImplementation : IStream
    {
        //string _file_path;
        //long _offset = 0;
        private MemoryStream _stream;

        public IStreamImplementation(string file_path)
        {
            using (var file_stream = File.OpenRead(file_path))
            {
                byte[] data = new byte[file_stream.Length];
                file_stream.Read(data, 0, (int)file_stream.Length);
                _stream = new MemoryStream(data);
            }
        }

        public void Clone(out IStream ppstm)
        {
            Console.WriteLine("IStream.Clone called");
            throw new NotImplementedException();
        }

        public void Commit(int grfCommitFlags)
        {
            Console.WriteLine("IStream.Commit called");
            throw new NotImplementedException();
        }

        public void CopyTo(IStream pstm, long cb, IntPtr pcbRead, IntPtr pcbWritten)
        {
            Console.WriteLine("IStream.CopyTo called");
            throw new NotImplementedException();
        }

        public void LockRegion(long libOffset, long cb, int dwLockType)
        {
            Console.WriteLine("IStream.LockRegion called");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads a specified number of bytes from the stream object into memory starting at the current 
        /// seek pointer.
        /// </summary>
        /// <param name="pv">A pointer to the buffer which the stream data is read into.</param>
        /// <param name="cb">The number of bytes of data to read from the stream object.</param>
        /// <param name="pcbRead">A pointer to a ULONG variable that receives the actual 
        /// number of bytes read from the stream object. </param>
        public void Read(byte[] pv, int cb, IntPtr pcbRead)
        {
            //Console.WriteLine("IStream.Read called");
            //Console.WriteLine("_offset:" + _offset);

            // Validate value of pcbRead
            if (Marshal.ReadInt64(pcbRead) != 0)
                throw new ArgumentException("I think pcbRead needs to be zero for things to work.");

            var result = _stream.Read(pv, 0, cb);
            Marshal.WriteInt64(pcbRead, result);
        }

        public void Revert()
        {
            Console.WriteLine("IStream.Revert called");
            throw new NotImplementedException();
        }

        /// <summary>
        /// Changes the seek pointer to a new location relative to the beginning of the stream, 
        /// the end of the stream, or the current seek pointer.
        /// </summary>
        /// <param name="dlibMove">The displacement to be added to the location indicated by 
        /// the dwOrigin parameter. If dwOrigin is STREAM_SEEK_SET, this is interpreted as an 
        /// unsigned value rather than a signed value.</param>
        /// <param name="dwOrigin">The origin for the displacement specified in dlibMove. The 
        /// origin can be the beginning of the file (STREAM_SEEK_SET), the current seek pointer 
        /// (STREAM_SEEK_CUR), or the end of the file (STREAM_SEEK_END). For more information 
        /// about values, see the STREAM_SEEK enumeration.</param>
        /// <param name="plibNewPosition">A pointer to the location where this method writes 
        /// the value of the new seek pointer from the beginning of the stream. You can set 
        /// this pointer to NULL. In this case, this method does not provide the new seek pointer.</param>
        public void Seek(long dlibMove, int dwOrigin, IntPtr plibNewPosition)
        {
            /*typedef enum tagSTREAM_SEEK { 
              STREAM_SEEK_SET  = 0,
              STREAM_SEEK_CUR  = 1,
              STREAM_SEEK_END  = 2
            } STREAM_SEEK;*/

            //Console.WriteLine("IStream.Seek called");

            if (dwOrigin == 0) // STREAM_SEEK_SET
            {
                _stream.Seek(dlibMove, SeekOrigin.Begin);
            }
            else if (dwOrigin == 1) // STREAM_SEEK_CUR
            {
                _stream.Seek(dlibMove, SeekOrigin.Current);
            }
            else if(dwOrigin == 2) // STREAM_SEEK_END
            {
                _stream.Seek(dlibMove, SeekOrigin.End);
            }

            Marshal.WriteInt64(plibNewPosition, _stream.Position);
        }

        public void SetSize(long libNewSize)
        {
            Console.WriteLine("IStream.SetSize called");
            throw new NotImplementedException();
        }

        public void Stat(out STATSTG pstatstg, int grfStatFlag)
        {
            //Console.WriteLine("IStream.Stat called");

            pstatstg = new STATSTG();
            pstatstg.cbSize = _stream.Length;
        }

        public void UnlockRegion(long libOffset, long cb, int dwLockType)
        {
            Console.WriteLine("IStream.UnlockRegion called");
            throw new NotImplementedException();
        }

        public void Write(byte[] pv, int cb, IntPtr pcbWritten)
        {
            Console.WriteLine("IStream.Write called");
            throw new NotImplementedException();
        }

        public long CurrentPosition
        {
            get { return _stream.Position;  }
        }
    }
}
