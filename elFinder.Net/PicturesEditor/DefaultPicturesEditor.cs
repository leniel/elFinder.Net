using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace ElFinder
{
    /// <summary>
    /// Represents default pictures editor
    /// </summary>
    internal class DefaultPicturesEditor : IPicturesEditor
    {
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set { _backgroundColor = value; }
        }

        public DefaultPicturesEditor(Color backgroundColor)
        {
            _backgroundColor = backgroundColor;
        }

        public DefaultPicturesEditor() : this(Color.Transparent) { }

        public ImageWithMime GenerateThumbnail(Stream input, int size, bool aspectRatio)
        {
            using (Image inputImage = Image.FromStream(input))
            {
                int width;
                int height;
                if (aspectRatio)
                {
                    double originalWidth = inputImage.Width;
                    double originalHeight = inputImage.Height;
                    double percentWidth = originalWidth != 0 ? size / originalWidth : 0;
                    double percentHeight = originalHeight != 0 ? size / originalHeight : 0;
                    double percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                    width = (int)(originalWidth * percent);
                    height = (int)(originalHeight * percent);
                }
                else
                {
                    width = size;
                    height = size;
                }
                return ScaleOrCrop(inputImage, new Rectangle(0, 0, inputImage.Width, inputImage.Height), new Rectangle(0, 0, width, height));
            }
        }

        public bool CanProcessFile(string fileExtension)
        {
            string ext = fileExtension.ToLower();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif" || ext == ".tiff";
        }

        public string ConvertThumbnailExtension(string originalImageExtension)
        {
            string ext = originalImageExtension.ToLower();
            if (ext == ".tiff")
                return ".png";
            if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                return ext;
            else
                throw new ArgumentException(typeof(DefaultPicturesEditor).FullName + " not support thumbnails for '" + originalImageExtension + "' file extension");
        }

        public void Resize(string file, int width, int height)
        {
            ImageWithMime output;
            using (Image inputImage = Image.FromFile(file))
            {
                output = ScaleOrCrop(inputImage, new Rectangle(0, 0, inputImage.Width, inputImage.Height), new Rectangle(0, 0, width, height));
            }
            using (FileStream outputFile = File.Create(file))
            {
                output.ImageStream.CopyTo(outputFile);
            }
        }

        public void Crop(string file, int x, int y, int width, int height)
        {
            ImageWithMime output;
            using (Image inputImage = Image.FromFile(file))
            {
                output = ScaleOrCrop(inputImage, new Rectangle(x, y, width, height), new Rectangle(0, 0, width, height));
            }
            using (FileStream outputFile = File.Create(file))
            {
                output.ImageStream.CopyTo(outputFile);
            }
        }

        public void Rotate(string file, int angle)
        {
            ImageWithMime output;
            using (Image inputImage = Image.FromFile(file))
            {
                output = Rotate(inputImage, angle);
            }
            using (FileStream outputFile = File.Create(file))
            {
                output.ImageStream.CopyTo(outputFile);
            }
        }

        private ImageWithMime ScaleOrCrop(Image inputImage, Rectangle src, Rectangle dst)
        {
            using (Bitmap newImage = new Bitmap(dst.Width, dst.Height))
            {
                using (Graphics gr = Graphics.FromImage(newImage))
                {
                    gr.SmoothingMode = SmoothingMode.HighQuality;
                    gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gr.DrawImage(inputImage, dst, src, GraphicsUnit.Pixel);
                }
                return SaveImage(newImage, inputImage.RawFormat);
            }
        }

        /// <summary>
        /// Creates a new Image containing the same image only rotated
        /// </summary>
        /// <param name="image">The <see cref="System.Drawing.Image"/> to rotate</param>
        /// <param name="angle">The amount to rotate the image, clockwise, in degrees</param>
        /// <returns>A new <see cref="System.Drawing.Bitmap"/> that is just large enough
        /// to contain the rotated image without cutting any corners off.</returns>
        /// <remarks>Original code can be found at http://www.codeproject.com/Articles/58815/C-Image-PictureBox-Rotations </remarks>
        /// <exception cref="System.ArgumentNullException">Thrown if <see cref="image"/> is null.</exception>
        private ImageWithMime Rotate(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            const double pi2 = Math.PI / 2.0;
            double oldWidth = (double)image.Width;
            double oldHeight = (double)image.Height;

            double theta = ((double)angle) * Math.PI / 180.0;
            double locked_theta = theta;

            
            while (locked_theta < 0.0)
                locked_theta += 2 * Math.PI;

            double newWidth, newHeight;
            int nWidth, nHeight; 

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
            }

            newWidth = adjacentTop + oppositeBottom;
            newHeight = adjacentBottom + oppositeTop;

            nWidth = (int)Math.Ceiling(newWidth);
            nHeight = (int)Math.Ceiling(newHeight);

            using (Bitmap rotatedBmp = new Bitmap(nWidth, nHeight))
            {
                using (Graphics g = Graphics.FromImage(rotatedBmp))
                {
                    g.Clear(_backgroundColor);
                    Point[] points;
                    if (locked_theta >= 0.0 && locked_theta < pi2)
                    {
                        points = new Point[] { 
											 new Point( (int) oppositeBottom, 0 ), 
											 new Point( nWidth, (int) oppositeTop ),
											 new Point( 0, (int) adjacentBottom )
										 };

                    }
                    else if (locked_theta >= pi2 && locked_theta < Math.PI)
                    {
                        points = new Point[] { 
											 new Point( nWidth, (int) oppositeTop ),
											 new Point( (int) adjacentTop, nHeight ),
											 new Point( (int) oppositeBottom, 0 )						 
										 };
                    }
                    else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                    {
                        points = new Point[] { 
											 new Point( (int) adjacentTop, nHeight ), 
											 new Point( 0, (int) adjacentBottom ),
											 new Point( nWidth, (int) oppositeTop )
										 };
                    }
                    else
                    {
                        points = new Point[] { 
											 new Point( 0, (int) adjacentBottom ), 
											 new Point( (int) oppositeBottom, 0 ),
											 new Point( (int) adjacentTop, nHeight )		
										 };
                    }
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;

                    g.DrawImage(image, points);
                }
                return SaveImage(rotatedBmp, image.RawFormat);
            }
        }

        private ImageWithMime SaveImage(Bitmap inputImage, ImageFormat imageFormat)
        {
            MemoryStream output = new MemoryStream();
            string mime;
            if (imageFormat.Guid == ImageFormat.Jpeg.Guid)
            {
                inputImage.Save(output, ImageFormat.Jpeg);
                mime = "image/jpeg";
            }
            else if (imageFormat.Guid == ImageFormat.Gif.Guid)
            {
                inputImage.Save(output, ImageFormat.Gif);
                mime = "image/gif";
            }
            else
            {
                inputImage.Save(output, ImageFormat.Png);
                mime = "image/png";
            }
            output.Position = 0;
            return new ImageWithMime(mime, output);
        }

        private Color _backgroundColor;       
    }
}
