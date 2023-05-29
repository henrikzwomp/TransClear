using System;
using System.Collections.Generic;
using System.Drawing;

namespace TransClear2
{
    public class ImageDownSizer // Yeah, the name is a little weird.
    {
        public static Bitmap ResizeImage(Bitmap source_image, int max_size)
        {
            Size size = CalculateNewSize(source_image.Width, source_image.Height, max_size);

            Bitmap result = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage((Image)result))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(source_image, 0, 0, size.Width, size.Height);
            }
            return result;
        }

        private static Size CalculateNewSize(int current_width, int current_height, int max_size)
        {
            if (current_width >= current_height)
            {
                int new_width = (int)((decimal)max_size / (decimal)current_width * (decimal)current_width);
                int new_height = (int)((decimal)max_size / (decimal)current_width * (decimal)current_height);
                return new Size(new_width, new_height);
            }
            else
            {
                int new_width = (int)((decimal)max_size / (decimal)current_height * (decimal)current_width);
                int new_height = (int)((decimal)max_size / (decimal)current_height * (decimal)current_height);
                return new Size(new_width, new_height);
            }
        }
    }
}
