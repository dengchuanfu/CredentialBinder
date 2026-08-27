using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

internal static class CreateIcon
{
    private static readonly int[] Sizes = { 16, 32, 48, 256 };

    private static void Main(string[] arguments)
    {
        if (arguments.Length != 2)
        {
            throw new ArgumentException("Usage: CreateIcon <source.png> <destination.ico>");
        }

        byte[][] images = new byte[Sizes.Length][];
        using (var source = new Bitmap(arguments[0]))
        {
            for (int index = 0; index < Sizes.Length; index++)
            {
                images[index] = ResizeToPng(source, Sizes[index]);
            }
        }

        using (var output = new BinaryWriter(File.Create(arguments[1])))
        {
            output.Write((ushort)0);
            output.Write((ushort)1);
            output.Write((ushort)images.Length);

            int offset = 6 + images.Length * 16;
            for (int index = 0; index < images.Length; index++)
            {
                int size = Sizes[index];
                output.Write((byte)(size == 256 ? 0 : size));
                output.Write((byte)(size == 256 ? 0 : size));
                output.Write((byte)0);
                output.Write((byte)0);
                output.Write((ushort)1);
                output.Write((ushort)32);
                output.Write(images[index].Length);
                output.Write(offset);
                offset += images[index].Length;
            }

            for (int index = 0; index < images.Length; index++)
            {
                output.Write(images[index]);
            }
        }
    }

    private static byte[] ResizeToPng(Image source, int size)
    {
        using (var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        using (var output = new MemoryStream())
        {
            graphics.Clear(Color.Transparent);
            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphics.DrawImage(source, new Rectangle(0, 0, size, size));
            bitmap.Save(output, ImageFormat.Png);
            return output.ToArray();
        }
    }
}
