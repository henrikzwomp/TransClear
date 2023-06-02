using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Drawing;
using System.IO;
//using System.IO.Compression;
using System.Runtime.InteropServices;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;

namespace TransClear2
{
    [ComVisible(true)]
    public class IoThumbnailHandler : IInitializeWithStream, IThumbnailProvider
    {
        public ComIStreamWrapper _source_stream { get; private set; }

        protected Bitmap GetThumbnailImage(uint width)
        {
            Bitmap image = null;

            using (ZipFile zip_file = new ZipFile(_source_stream))
            {
                zip_file.Password = "soho0909";

                var png_file_entry = zip_file.GetEntry("thumbnail.png");
                var png_file_stream = zip_file.GetInputStream(png_file_entry);

                image = new Bitmap(png_file_stream);
            }

            if (image.Width > width || image.Height > width)
            {
                image = ImageDownSizer.ResizeImage(image, (int)width);
            }

            return image;
        }

        /// <summary>
        /// Gets a thumbnail image and alpha type.
        /// </summary>
        /// <param name="cx">The maximum thumbnail size, in pixels.</param>
        /// <param name="phbmp">Pointer to the thumbnail image handle.</param>
        /// <param name="pdwAlpha">Pointer to the WTS_ALPHATYPE enumeration.</param>
        /// <returns>
        /// If this method succeeds, it returns S_OK. Otherwise, it returns an HRESULT error code.
        /// </returns>
        int IThumbnailProvider.GetThumbnail(uint cx, out IntPtr phbmp, out WTS_ALPHATYPE pdwAlpha)
        {
            phbmp = IntPtr.Zero;
            pdwAlpha = WTS_ALPHATYPE.WTSAT_UNKNOWN;
            Bitmap thumbnailImage;

            try
            {
                thumbnailImage = GetThumbnailImage(cx);
            }
            catch (Exception e)
            {
                //logger.LogError(e);
                return WinError.E_FAIL;
            }

            if (thumbnailImage == null)
            {
                //  DebugLog a warning return failure.
                //Log("The internal GetThumbnail function failed to return a valid thumbnail.");
                return WinError.E_FAIL;
            }

            phbmp = thumbnailImage.GetHbitmap();
            pdwAlpha = WTS_ALPHATYPE.WTSAT_ARGB;

            _source_stream.ReleaseStream();

            return WinError.S_OK;
        }

        /// <summary>
        /// Initializes a handler with a stream.
        /// </summary>
        /// <param name="pstream">A pointer to an IStream interface that represents the stream source.</param>
        /// <param name="grfMode">One of the following STGM values that indicates the access mode for pstream. STGM_READ or STGM_READWRITE.</param>
        public int Initialize(IStream pstream, uint grfMode)
        {
            //  Set the selected item stream.
            _source_stream = new ComIStreamWrapper(pstream);

            //  Return success.
            return WinError.S_OK;
        }

    }

    
}
