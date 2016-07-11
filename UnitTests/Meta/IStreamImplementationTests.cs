using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;

namespace UnitTests.Meta
{
    [TestFixture]
    public class IStreamImplementationTests
    {
        private IntPtr _long_pointer;

        [SetUp]
        public void SetUp()
        {
            _long_pointer = Marshal.AllocCoTaskMem(8);
            Marshal.WriteInt64(_long_pointer, 0);
        }

        [TearDown]
        public void TearDown()
        {
            Marshal.FreeCoTaskMem(_long_pointer);
        }

        [Test]
        public void CanReadASmallPart()
        {
            var test = new IStreamImplementation(@"TestFiles\TestTextData.txt");

            byte[] result = new byte[10];

            test.Read(result, 10, _long_pointer);

            long amount_read = Marshal.ReadInt64(_long_pointer);

            Assert.That(amount_read, Is.EqualTo(10));
            Assert.That(result[0], Is.EqualTo(48));
            Assert.That(result[1], Is.EqualTo(49));
            Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("0123456789"));

        }

        [Test]
        public void CanGetLength()
        {
            var test = new IStreamImplementation(@"TestFiles\TestTextData.txt");

            STATSTG pstatstg = new STATSTG();

            test.Stat(out pstatstg, 0);

            Assert.That(pstatstg.cbSize, Is.EqualTo(22));
            
        }

        [Test]
        public void CanReadASmallPartFromCurrentPosition()
        {
            var test = new IStreamImplementation(@"TestFiles\TestTextData.txt");
            // 11
            byte[] result = new byte[11];

            test.Seek(11, 0, _long_pointer);
            Marshal.WriteInt64(_long_pointer, 0);
            test.Read(result, 11, _long_pointer);

            //Assert.That(result[0], Is.EqualTo(48));
            //Assert.That(result[1], Is.EqualTo(49));
            Assert.That(Encoding.UTF8.GetString(result), Is.EqualTo("Hello World"));
        }

        [Test]
        public void CanSeek()
        {
            var test_object = new IStreamImplementation(@"TestFiles\TestTextData.txt");

            test_object.Seek(3, 0, _long_pointer); // 3 steps from beginning

            Assert.That(Marshal.ReadInt64(_long_pointer), Is.EqualTo(3));

            test_object.Seek(2, 1, _long_pointer); // 2 steps from current

            Assert.That(Marshal.ReadInt64(_long_pointer), Is.EqualTo(5));

            test_object.Seek(-5, 2, _long_pointer); // 5 steps from end

            Assert.That(Marshal.ReadInt64(_long_pointer), Is.EqualTo(22 - 5));
        }
    }
}
