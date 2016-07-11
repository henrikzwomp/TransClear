using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using LxfHandler;
using System.Drawing;

namespace UnitTests.LxfHandler
{
    [TestFixture]
    public class ImageDownSizerTests
    {
        [Test]
        public void CanMakeImageSmaller()
        {
            var input_bitmap = new Bitmap(256, 256);

            var result = ImageDownSizer.ResizeImage(input_bitmap, 128);

            Assert.That(result.Width, Is.EqualTo(128));
            Assert.That(result.Height, Is.EqualTo(128));
        }

        [Test]
        public void CanHandleLandscapeImage()
        {
            var input_bitmap = new Bitmap(256, 128);

            var result = ImageDownSizer.ResizeImage(input_bitmap, 128);

            Assert.That(result.Width, Is.EqualTo(128));
            Assert.That(result.Height, Is.EqualTo(64));
        }

        [Test]
        public void CanHandlePortraitImage()
        {
            var input_bitmap = new Bitmap(128, 256);

            var result = ImageDownSizer.ResizeImage(input_bitmap, 128);

            Assert.That(result.Width, Is.EqualTo(64));
            Assert.That(result.Height, Is.EqualTo(128));
        }
    }
}
