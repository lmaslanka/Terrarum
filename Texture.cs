namespace Terrarum
{
    using System.Collections.Generic;
    using OpenTK.Graphics.OpenGL;
    using SixLabors.ImageSharp;
    using SixLabors.ImageSharp.PixelFormats;
    using SixLabors.ImageSharp.Processing;
    using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

    public class Texture
    {
        public readonly int Handle;

        // Create texture from path.
        public Texture (string path)
        {
            // Generate handle
            Handle = GL.GenTexture ();

            // Bind the handle
            Use ();

            // For this example, we're going to use .NET's built-in System.Drawing library to load textures.

            // Load the image
            //Load the image
            Image<Rgba32> image = (Image<Rgba32>)Image.Load(path);

            //ImageSharp loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            //This will correct that, making the texture display properly.
            image.Mutate(x => x.Flip(FlipMode.Vertical));

            List<byte> pixels = new List<byte>();

            //Get an array of the pixels, in ImageSharp's internal format.
            for(var y = 0; y < image.Height; y++)
            {
                Rgba32[] tempPixels = image.GetPixelRowSpan(y).ToArray();

                for(var x = 0; x < tempPixels.Length; x++)
                {
                    pixels.Add(tempPixels[x].R);
                    pixels.Add(tempPixels[x].G);
                    pixels.Add(tempPixels[x].B);
                    pixels.Add(tempPixels[x].A);
                }
            }
            

            // Now that our pixels are prepared, it's time to generate a texture. We do this with GL.TexImage2D
            // Arguments:
            //   The type of texture we're generating. There are various different types of textures, but the only one we need right now is Texture2D.
            //   Level of detail. We can use this to start from a smaller mipmap (if we want), but we don't need to do that, so leave it at 0.
            //   Target format of the pixels. This is the format OpenGL will store our image with.
            //   Width of the image
            //   Height of the image.
            //   Border of the image. This must always be 0; it's a legacy parameter that Khronos never got rid of.
            //   The format of the pixels, explained above. Since we loaded the pixels as ARGB earlier, we need to use BGRA.
            //   Data type of the pixels.
            //   And finally, the actual pixels.
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixels.ToArray());

            // Now that our texture is loaded, we can set a few settings to affect how the image appears on rendering.

            // First, we set the min and mag filter. These are used for when the texture is scaled down and up, respectively.
            // Here, we use Linear for both. This means that OpenGL will try to blend pixels, meaning that textures scaled too far will look blurred.
            // You could also use (amongst other options) Nearest, which just grabs the nearest pixel, which makes the texture look pixelated if scaled too far.
            // NOTE: The default settings for both of these are LinearMipmap. If you leave these as default but don't generate mipmaps,
            // your image will fail to render at all (usually resulting in pure black instead).
            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Linear);
            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Linear);

            // Now, set the wrapping mode. S is for the X axis, and T is for the Y axis.
            // We set this to Repeat so that textures will repeat when wrapped. Not demonstrated here since the texture coordinates exactly match
            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) TextureWrapMode.Repeat);
            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) TextureWrapMode.Repeat);

            // Next, generate mipmaps.
            // Mipmaps are smaller copies of the texture, scaled down. Each mipmap level is half the size of the previous one
            // Generated mipmaps go all the way down to just one pixel.
            // OpenGL will automatically switch between mipmaps when an object gets sufficiently far away.
            // This prevents distant objects from having their colors become muddy, as well as saving on memory.
            GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);
        }

        // Activate texture
        // Multiple textures can be bound, if your shader needs more than just one.
        // If you want to do that, use GL.ActiveTexture to set which slot GL.BindTexture binds to.
        // The OpenGL standard requires that there be at least 16, but there can be more depending on your graphics card.
        public void Use (TextureUnit unit = TextureUnit.Texture0)
        {
            GL.ActiveTexture (unit);
            GL.BindTexture (TextureTarget.Texture2D, Handle);
        }
    }
}