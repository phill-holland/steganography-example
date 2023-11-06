using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Steganography
{
    class Program
    {
        static void Encode(string filename, string message)
        {
            Image<Rgba32> source = Image.Load<Rgba32>(filename);

            if(source.Width * source.Height < message.Length) return;

            byte[] data = Encoding.ASCII.GetBytes(message);
            int index = 0;

            using(Image<Rgba32> dest = new Image<Rgba32>(source.Width, source.Height))
            {
                for(int y = 0; y < source.Height; ++y)
                {
                    for(int x = 0; x < source.Width; ++x)
                    {
                        Rgba32 pixel = source[x,y];
                        if(index == 0) 
                        {
                            // encode size of message in first pixel                            
                            UInt32 temp = (UInt32)data.Length;
                            
                            pixel.R = (byte)(temp & 0xFF);
                            pixel.G = (byte)((temp >> 8) & 0xFF);
                            pixel.B = (byte)((temp >> 16) & 0xFF);
                            pixel.A = (byte)((temp >> 24) & 0xFF);
                        }
                        else if(index <= message.Length)
                        {
                            // encode single ASCII character in LSB colours of pixel
                            byte value = data[index - 1];

                            // clear lowest two bits per pixel
                            pixel.R = (byte)(pixel.R & 0xFC);
                            pixel.G = (byte)(pixel.G & 0xFC);
                            pixel.B = (byte)(pixel.B & 0xFC);
                            pixel.A = (byte)(pixel.A & 0xFC);

                            // encode character into lowest two bit of each pixel
                            pixel.R = (byte)(pixel.R | (value & (0x3)));
                            pixel.G = (byte)(pixel.G | (value >> 2 & (0x3)));
                            pixel.B = (byte)(pixel.B | (value >> 4 & (0x3)));
                            pixel.A = (byte)(pixel.A | (value >> 6 & (0x3)));
                        }

                        dest[x,y] = pixel;

                        ++index;
                    }
                }

                dest.SaveAsPng("output.png");
            }
        }

        static string Decode(string filename)
        {
            Image<Rgba32> source = Image.Load<Rgba32>(filename);

            Rgba32 first = source[0,0];

            int size = 0;

            // decode size from first pixel
            size |= (first.R & 0xFF);
            size |= (first.G << 8);
            size |= (first.B << 16);
            size |= (first.A << 24);

            if(size >= source.Width * source.Height) return "";

            byte []data = new byte[size];
            int index = 0;

            int y = 0;
            int x = 1;
            for(int i = 1; i <= size; i++)
            {
                Rgba32 pixel = source[x,y];

                byte value = 0;

                // decode ASCII characters from pixels and recombine
                value |= (byte)(pixel.R & 0x3);
                value |= (byte)(((pixel.G & 0x3) << 2) & 0xC);
                value |= (byte)(((pixel.B & 0x3) << 4) & 0x30);
                value |= (byte)(((pixel.A & 0x3) << 6) & 0xC0);

                data[index++] = value;

                ++x;
                if(x >= source.Width)
                {
                    x = 0;
                    ++y;
                }
            }

            return Encoding.ASCII.GetString(data);
        }

        static void Main(string[] args)
        {
            Encode("images/cake.png", "hello world!");
            Console.WriteLine(Decode("output.png"));            
        }
    };
};