# Steganography Demonstration

This is a simple dotnet core console application demonstration encoding simple messages within images (png files)

# Method

A 32 bit colour png image is loaded, and then within the output the first pixel is then used to encode the size of the message;

```
    UInt32 temp = (UInt32)message.Length;
    
    pixel.R = (byte)(temp & 0xFF);
    pixel.G = (byte)((temp >> 8) & 0xFF);
    pixel.B = (byte)((temp >> 16) & 0xFF);
    pixel.A = (byte)((temp >> 24) & 0xFF);
```

The size can be any 32 bit unsigned integer, which then using bit shifting, is split up and divided between the four colours of the pixel (red,green,blue and alpha)

The message is then converted to ASCII values, and each 255 ASCII byte value, is spread across the lowest two bits, of each colour of the destination pixel.

```
    pixel.R = (byte)(pixel.R | (value & (0x3)));
    pixel.G = (byte)(pixel.G | (value >> 2 & (0x3)));
    pixel.B = (byte)(pixel.B | (value >> 4 & (0x3)));
    pixel.A = (byte)(pixel.A | (value >> 6 & (0x3)));
```

Using this method reduces any chance of significantly changing the colour of any given pixel, the change should be generally be perceivable by the naked eye.

# Requirements

- dotnet core SDK 6 or above installed

# Running

at the command prompt, type;

```
dotnet run
```



