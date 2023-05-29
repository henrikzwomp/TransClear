using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using TransClear2;
using System.IO.Compression;
using System.IO;

using System.Runtime.InteropServices.ComTypes;
using System.Drawing;


namespace UnitTests.TransClear2
{
    [TestFixture]
    public class ComIStreamWrapperTests
    {
        [Test]
        public void CanReadASmallPart()
        {
            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            var wrapper = new ComIStreamWrapper(istream_object);

            byte[] result = new byte[10];
            wrapper.Read(result, 0, 10);

            Assert.That(result[0], Is.EqualTo(48));
            Assert.That(result[1], Is.EqualTo(49));
            Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("0123456789"));
        }

        [Test]
        public void CanGetLength()
        {
            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            var wrapper = new ComIStreamWrapper(istream_object);

            Assert.That(wrapper.Length, Is.EqualTo(22));
        }

        [Test]
        public void CanOffSetWhereItStoresDataInBuffer()
        {
            // 0123456789 Hello World
            // 0123456789012345678901

            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            var wrapper = new ComIStreamWrapper(istream_object);

            byte[] result = new byte[10];

            wrapper.Seek(-5, SeekOrigin.End);
            wrapper.Read(result, 5, 5);

            Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("\0\0\0\0\0World"));
            Assert.That(wrapper.Position, Is.EqualTo(22));
        }

        [Test]
        public void CanSeekFromEnd()
        {
            var istream = new IStreamImplementation(@"TestFiles\TransClear3003.lxf"); // ISteam
            var wrapper = new ComIStreamWrapper(istream);

            int offset = -18;
            long length = wrapper.Length;

            var result = wrapper.Seek(offset, SeekOrigin.End);

            Assert.That(result, Is.EqualTo(length + offset));
        }

        [Test]
        public void CanMakeShallowRead()
        {
            var istream = new IStreamImplementation(@"TestFiles\TransClear3003.lxf"); // ISteam
            var wrapper = new ComIStreamWrapper(istream);

            ZipArchiveEntry lxfml_file = null;
            ZipArchiveEntry png_file = null;

            ZipArchive archive = new ZipArchive(wrapper); // new SpyStreamWrapper(wrapper)
            lxfml_file = archive.GetEntry("IMAGE100.LXFML");
            png_file = archive.GetEntry("IMAGE100.PNG");
            Assert.That(lxfml_file, Is.Not.Null);
            Assert.That(lxfml_file, Is.Not.Null);
        }

        [Test]
        public void CantReadBeyondEndOfStream()
        {
            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            var wrapper = new ComIStreamWrapper(istream_object);

            byte[] result = new byte[10];

            wrapper.Seek(-5, SeekOrigin.End);
            Assert.Throws<ArgumentException>(() => wrapper.Read(result, 0, 10));
        }

        [Test]
        public void CanReturnRightNumberOfReadBytes()
        {
            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            var wrapper = new ComIStreamWrapper(istream_object);

            byte[] result = new byte[5];
            int count = wrapper.Read(result, 0, 5);

            Assert.That(count, Is.EqualTo(5));
        }

        [Test, Explicit]
        public void JustCheckingThatTestFileCanBeRead()
        {
            using (var file_stream = File.OpenRead(@"TestFiles\TransClear3003.lxf"))
            {
                byte[] pv = new byte[32];
                file_stream.Position = 5618;
                var result = file_stream.Read(pv, 0, 32);
            }
        }

        [Test, Explicit] // ExpectedException
        public void JustCheckingBehaivor()
        {
            var stream = File.OpenRead(@"TestFiles\TestTextData.txt");

            byte[] result = new byte[11];
            int count = stream.Read(result, 17, 11);

            Assert.That(count, Is.EqualTo(5));
            Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("World"));
        }

        [Test, Explicit("Can break Nunit :/")]
        public void CanReleaseStreamObject()
        {
            var istream_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");

            var weak_ref = new WeakReference(istream_object);

            var wrapper = new ComIStreamWrapper(istream_object);

            istream_object = null;
            GC.Collect();
            
            Assert.That(weak_ref.IsAlive, Is.True);

            wrapper.ReleaseStream();
            wrapper = null;

            GC.Collect();

            Assert.That(weak_ref.IsAlive, Is.False);
        }
    }
}