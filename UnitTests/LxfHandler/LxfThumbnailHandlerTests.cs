using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using LxfHandler;

//using System.Runtime.InteropServices.ComTypes; // IStream
//using System.IO;
using System.Drawing;

namespace UnitTests.LxfHandler
{
    [TestFixture]
    public class LxfThumbnailHandlerTests
    {
        [Test]
        public void CanGenerateThumbnailFromFile()
        {
            uint cx = 128;
            IntPtr phbmp = new IntPtr();
            WTS_ALPHATYPE pdwAlpha = WTS_ALPHATYPE.WTSAT_UNKNOWN;

            Assert.That(phbmp.ToInt64(), Is.EqualTo(0));

            var pstream = new IStreamImplementation(@"TestFiles\TransClear3003.lxf"); // ISteam
            uint grfMode = 0; // This value is not used in code

            var handler = new LxfThumbnailHandler();

            handler.Initialize(pstream, grfMode);

            int result = ((IThumbnailProvider) handler).GetThumbnail(cx, out phbmp, out pdwAlpha);

            Assert.That(result, Is.EqualTo(WinError.S_OK));
            Assert.That(pdwAlpha, Is.EqualTo(WTS_ALPHATYPE.WTSAT_ARGB));
            Assert.That(phbmp.ToInt64(), Is.Not.EqualTo(0));

            Bitmap bmp = Bitmap.FromHbitmap(phbmp);
            Assert.That(bmp.Width, Is.EqualTo(cx));
        }
        
        [Test]
        public void CanGenerateASmallerThumbnail()
        {
            uint cx = 16;
            IntPtr phbmp = new IntPtr();
            WTS_ALPHATYPE pdwAlpha = WTS_ALPHATYPE.WTSAT_UNKNOWN;

            Assert.That(phbmp.ToInt64(), Is.EqualTo(0));

            var pstream = new IStreamImplementation(@"TestFiles\TransClear3003.lxf"); // ISteam
            uint grfMode = 0; // This value is not used in code

            var handler = new LxfThumbnailHandler();

            handler.Initialize(pstream, grfMode);

            int result = ((IThumbnailProvider)handler).GetThumbnail(cx, out phbmp, out pdwAlpha);

            Assert.That(result, Is.EqualTo(WinError.S_OK));
            Assert.That(pdwAlpha, Is.EqualTo(WTS_ALPHATYPE.WTSAT_ARGB));
            Assert.That(phbmp.ToInt64(), Is.Not.EqualTo(0));

            Bitmap bmp = Bitmap.FromHbitmap(phbmp);
            Assert.That(bmp.Width, Is.EqualTo(cx));
        }

        [Test, Explicit]
        public void WillReleaseStreamObject()
        {
            throw new NotImplementedException();
        }

        [Test, Explicit]
        public void WillReleaseStreamObjectEvenWhenItFails()
        {
            throw new NotImplementedException();
        }
    }

}
