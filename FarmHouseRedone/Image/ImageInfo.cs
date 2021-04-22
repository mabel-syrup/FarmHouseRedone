using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarmHouseRedone.Image
{
    public class ImageInfo
    {
        public int Width;
        public int Height;
        public Color[] colorInfo;

        public ImageInfo R => channel(0, false);

        public ImageInfo G => channel(1, false);

        public ImageInfo B => channel(2, false);

        public ImageInfo(Texture2D source)
        {
            colorInfo = new Color[source.Width * source.Height];
            Width = source.Width;
            Height = source.Height;
            for (var x = 0; x < source.Width; x++)
            for (var y = 0; y < source.Height; y++)
                colorInfo[x + y * source.Width] = getPixel(source, x, y);
        }

        public ImageInfo(Color[] data, int width, int height)
        {
            Width = width;
            Height = height;
            colorInfo = data;
        }

        public ImageInfo(Color background, int width, int height)
        {
            Width = width;
            Height = height;
            colorInfo = new Color[width * height];
            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                colorInfo[x + y * width] = background;
        }

        public Texture2D render()
        {
            var outTexture = new Texture2D(Game1.graphics.GraphicsDevice, Width, Height, true, SurfaceFormat.Color);
            outTexture.SetData(colorInfo);
            return outTexture;
        }

        public static Color getPixel(Texture2D texture, int x, int y)
        {
            x = (x + texture.Width) % texture.Width;
            y = (y + texture.Height) % texture.Height;

            var buffer = new byte[4];
            try
            {
                texture.GetData<byte>(0, new Rectangle(x, y, 1, 1), buffer, 0, 4);

                var outColor = new Color((int) buffer[0], (int) buffer[1], (int) buffer[2], (int) buffer[3]);
                return outColor;
            }
            catch (ArgumentException)
            {
                Logger.Log("Rectangle (" + x + ", " + y + ", 1, 1) was invalid!", StardewModdingAPI.LogLevel.Error);
                return Color.Black;
            }
        }

        public Color getPixel(int x, int y)
        {
            x = (x + Width) % Width;
            y = (y + Height) % Height;

            return colorInfo[x + y * Width];
        }

        public static Color getPixel(ImageInfo image, int x, int y)
        {
            x = (x + image.Width) % image.Width;
            y = (y + image.Height) % image.Height;

            return image.colorInfo[x + y * image.Width];
        }

        public void setPixel(int x, int y, Color c)
        {
            x = (x + Width) % Width;
            y = (y + Height) % Height;

            colorInfo[x + y * Width] = c;
        }

        public ImageInfo mirror(bool horizontal)
        {
            var width = horizontal ? Width * 2 : Width;
            var height = horizontal ? Height : Height * 2;

            var data = new Color[width * height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);
                if (horizontal)
                {
                    data[x + y * width] = pixel;
                    data[width - 1 - x + y * width] = pixel;
                }
                else
                {
                    data[x + y * width] = pixel;
                    data[x + (height - 1 - y) * width] = pixel;
                }
            }

            return new ImageInfo(data, width, height);
        }

        public ImageInfo multiply(ImageInfo b, float factor)
        {
            float xScaleFactor = b.Width / Width;
            float yScaleFactor = b.Height / Height;

            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var aPixel = getPixel(this, x, y);
                var bPixel = getPixel(b, (int) (x * xScaleFactor), (int) (y * yScaleFactor));

                //float aAverage = (int)((aPixel.R + aPixel.G + aPixel.B) / 3f) / 256f;
                //float bAverage = (int)((bPixel.R + bPixel.G + bPixel.B) / 3f) / 256f;

                //int multiplied = (int)(aAverage * bAverage * 256);

                var rM = (int) (aPixel.R / 256f * (bPixel.R / 256f) * 256f);
                var gM = (int) (aPixel.G / 256f * (bPixel.G / 256f) * 256f);
                var bM = (int) (aPixel.B / 256f * (bPixel.B / 256f) * 256f);

                outData[x + y * Width] = new Color(rM, gM, bM, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo divide(ImageInfo b)
        {
            float xScaleFactor = b.Width / Width;
            float yScaleFactor = b.Height / Height;

            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var aPixel = getPixel(this, x, y);
                var bPixel = getPixel(b, (int) (x * xScaleFactor), (int) (y * yScaleFactor));

                //float aAverage = (int)((aPixel.R + aPixel.G + aPixel.B) / 3f) / 256f;
                //float bAverage = (int)((bPixel.R + bPixel.G + bPixel.B) / 3f) / 256f;

                //int multiplied = (int)(aAverage * bAverage * 256);
                int rM;
                int gM;
                int bM;
                if (bPixel.R > 0)
                    rM = (int) (aPixel.R / 256f / (bPixel.R / 256f) * 256f);
                else
                    rM = 255;
                if (bPixel.G > 0)
                    gM = (int) (aPixel.G / 256f / (bPixel.G / 256f) * 256f);
                else
                    gM = 255;
                if (bPixel.B > 0)
                    bM = (int) (aPixel.B / 256f / (bPixel.B / 256f) * 256f);
                else
                    bM = 255;

                outData[x + y * Width] = new Color(rM, gM, bM, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo multiply(double amount)
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);

                var rM = (int) Math.Min(Math.Max(pixel.R * amount, 0), 255);
                var gM = (int) Math.Min(Math.Max(pixel.G * amount, 0), 255);
                var bM = (int) Math.Min(Math.Max(pixel.B * amount, 0), 255);

                outData[x + y * Width] = new Color(rM, gM, bM, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo toGreyScale(bool average = true)
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);

                var grey = 0;
                if (average)
                    grey = (int) ((pixel.R + pixel.G + pixel.B) / 3f);
                else
                    grey = Math.Max(pixel.R, Math.Max(pixel.G, pixel.G));
                outData[x + y * Width] = new Color(grey, grey, grey, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo divide(double denominator)
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);

                var average = (int) ((pixel.R + pixel.G + pixel.B) / 3f) / 256f;

                var divided = (int) ((float) average / (float) denominator * 256);
                outData[x + y * Width] = new Color(divided, divided, divided, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo add(double amount)
        {
            var outData = new Color[Width * Height];

            if (Math.Abs(amount) >= 1.0)
                amount = amount / 256.0;

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);

                //float average = (int)((pixel.R + pixel.G + pixel.B) / 3f) / 256f;
                var r = Math.Min(255, pixel.R + (int) (amount * 256));
                var g = Math.Min(255, pixel.G + (int) (amount * 256));
                var b = Math.Min(255, pixel.B + (int) (amount * 256));

                //int added = Math.Min((int)((average + amount) * 256), 255);
                outData[x + y * Width] = new Color(r, g, b, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo maximize(bool asGreyScale = false)
        {
            var outData = new Color[Width * Height];

            var highest = 0;
            var lowest = 255;

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);
                if (asGreyScale)
                {
                    highest = Math.Max((int) ((pixel.R + pixel.G + pixel.B) / 3f), highest);
                    lowest = Math.Min((int) ((pixel.R + pixel.G + pixel.B) / 3f), lowest);
                }
                else
                {
                    highest = Math.Max(Math.Max(pixel.R, pixel.G), Math.Max(pixel.B, highest));
                    lowest = Math.Min(Math.Min(pixel.R, pixel.G), Math.Min(pixel.B, lowest));
                }
            }

            var range = highest - lowest;

            if (range <= 0)
                return this;

            var factor = 256.0 / (double) range;
            Logger.Log("Maximization factor: " + factor + " (256 / (" + highest + " - " + lowest + "))");

            return (this - lowest) * factor;
        }

        public ImageInfo add(ImageInfo imageB)
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixelA = getPixel(this, x, y);
                var pixelB = getPixel(imageB, x, y);

                var r = Math.Min(pixelA.R + pixelB.R, 255);
                var g = Math.Min(pixelA.G + pixelB.G, 255);
                var b = Math.Min(pixelA.B + pixelB.B, 255);
                outData[x + y * Width] = new Color(r, g, b, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo channel(int channel, bool toGreyscale)
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixel = getPixel(this, x, y);

                int channelValue = channel == 0 ? pixel.R : channel == 1 ? pixel.G : channel == 2 ? pixel.B : pixel.A;
                if (toGreyscale)
                    outData[x + y * Width] = new Color(channelValue, channelValue, channelValue, 255);
                else
                    outData[x + y * Width] = new Color(channel == 0 ? channelValue : 0, channel == 1 ? channelValue : 0,
                        channel == 2 ? channelValue : 0, channel > 3 ? channelValue : 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo proximityMap(int falloff = 4)
        {
            var valueSize = falloff * 2 + 1;
            var valueGrid = new double[valueSize, valueSize];
            for (var x = 0; x < valueSize; x++)
                //string column = "[";
            for (var y = 0; y < valueSize; y++)
            {
                var value = Math.Sqrt(Math.Pow(x - falloff, 2) + Math.Pow(y - falloff, 2));
                valueGrid[x, y] = value;
                //column += Math.Round(value,3) + (y == valueSize - 1 ? "]" : ", ");
            }
            //Logger.Log(column);

            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                double smallest = falloff + 1;
                for (var xV = -falloff; xV < valueSize - falloff; xV++)
                {
                    var polledX = x + xV;
                    if (polledX < 0)
                        continue;
                    if (polledX >= Width)
                        break;
                    for (var yV = -falloff; yV < valueSize - falloff; yV++)
                    {
                        var polledY = y + yV;
                        if (polledY < 0)
                            continue;
                        if (polledY >= Height)
                            break;
                        //Logger.Log("Pixel [" + polledX + ", " + polledY + "] is on the map");
                        var pixel = getPixel(this, polledX, polledY);
                        if (pixel.R > 0 || pixel.G > 0 || pixel.B > 0)
                            smallest = Math.Min(valueGrid[xV + falloff, yV + falloff], smallest);
                        //else
                        //    Logger.Log("Pixel [" + polledX + ", " + polledY + "] is black");
                    }
                }

                var proximityValue = 0;
                //Logger.Log("Smallest value for [" + x + ", " + y + "] is " + smallest);
                if (smallest < falloff)
                    proximityValue = (int) ((falloff - smallest) / falloff * 256);
                outData[x + y * Width] = new Color(proximityValue, proximityValue, proximityValue, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo invert()
        {
            var outData = new Color[Width * Height];

            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pixelA = getPixel(this, x, y);

                var r = 255 - pixelA.R;
                var g = 255 - pixelA.G;
                var b = 255 - pixelA.B;
                outData[x + y * Width] = new Color(r, g, b, 255);
            }

            return new ImageInfo(outData, Width, Height);
        }

        public ImageInfo append(ImageInfo b, Vector2 position)
        {
            return null;
        }

        public ImageInfo crop(Rectangle newCanvas)
        {
            return crop(newCanvas, Color.Transparent);
        }

        public ImageInfo crop(Rectangle newCanvas, Color background)
        {
            var outData = new Color[newCanvas.Width * newCanvas.Height];

            var sourceBounds = new Rectangle(0, 0, Width, Height);

            for (var x = 0; x < newCanvas.Width; x++)
            for (var y = 0; y < newCanvas.Height; y++)
            {
                var xSource = x + newCanvas.X;
                var ySource = y + newCanvas.Y;

                Color pixel;
                if (sourceBounds.Contains(xSource, ySource))
                    pixel = getPixel(this, xSource, ySource);
                else
                    pixel = background;

                outData[x + y * newCanvas.Width] = pixel;
            }

            return new ImageInfo(outData, newCanvas.Width, newCanvas.Height);
        }

        public static explicit operator Texture2D(ImageInfo a)
        {
            return a.render();
        }

        public static implicit operator ImageInfo(Texture2D a)
        {
            return new ImageInfo(a);
        }

        public static ImageInfo operator !(ImageInfo a)
        {
            return a.invert();
        }

        public static ImageInfo operator +(ImageInfo a)
        {
            return a;
        }

        public static ImageInfo operator +(ImageInfo a, ImageInfo b)
        {
            return a.add(b);
        }

        public static ImageInfo operator +(ImageInfo a, double b)
        {
            return a.add(b);
        }

        public static ImageInfo operator -(ImageInfo a)
        {
            return a.invert();
        }

        public static ImageInfo operator -(ImageInfo a, ImageInfo b)
        {
            return a.add(-b);
        }

        public static ImageInfo operator -(ImageInfo a, double b)
        {
            return a.add(-b);
        }

        public static ImageInfo operator *(ImageInfo a, ImageInfo b)
        {
            return a.multiply(b, 1f);
        }

        public static ImageInfo operator *(ImageInfo a, double b)
        {
            return a.multiply(b);
        }

        public static ImageInfo operator /(ImageInfo a, ImageInfo b)
        {
            return a.divide(b);
        }

        public static ImageInfo operator /(ImageInfo a, double b)
        {
            return a.divide(b);
        }
    }
}