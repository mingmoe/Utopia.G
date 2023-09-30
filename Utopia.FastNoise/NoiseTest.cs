#region copyright
// This file(may named NoiseTest.cs) is a part of the project: Utopia.FastNoise.
// 
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// 
// This file is part of Utopia.FastNoise.
//
// Utopia.FastNoise is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// Utopia.FastNoise is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License along with Utopia.FastNoise. If not, see <https://www.gnu.org/licenses/>.
#endregion

using Utopia.FastNoise;

internal static class BitmapGenerator
{
#pragma warning disable IDE0060 // Remove unused parameter
    static void Main(string[] args)
#pragma warning restore IDE0060 // Remove unused parameter
    {
#pragma warning disable IDE0007 // Use implicit type
        FastNoise cellular = new FastNoise("CellularDistance");
#pragma warning restore IDE0007 // Use implicit type
        cellular.Set("ReturnType", "Index0Add1");
        cellular.Set("DistanceIndex0", 2);

#pragma warning disable IDE0007 // Use implicit type
        FastNoise fractal = new FastNoise("FractalFBm");
#pragma warning restore IDE0007 // Use implicit type
        fractal.Set("Source", new FastNoise("Simplex"));
        fractal.Set("Gain", 0.3f);
        fractal.Set("Lacunarity", 0.6f);

#pragma warning disable IDE0007 // Use implicit type
        FastNoise addDim = new FastNoise("AddDimension");
#pragma warning restore IDE0007 // Use implicit type
        addDim.Set("Source", cellular);
        addDim.Set("NewDimensionPosition", 0.5f);
        // or
        // addDim.Set("NewDimensionPosition", new FastNoise("Perlin"));

#pragma warning disable IDE0007 // Use implicit type
        FastNoise maxSmooth = new FastNoise("MaxSmooth");
#pragma warning restore IDE0007 // Use implicit type
        maxSmooth.Set("LHS", fractal);
        maxSmooth.Set("RHS", addDim);

        Console.WriteLine("SIMD Level " + maxSmooth.GetSIMDLevel());

        GenerateBitmap(maxSmooth, "testMetadata");

        // Simplex fractal ENT
#pragma warning disable IDE0007 // Use implicit type
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
        FastNoise nodeTree = FastNoise.FromEncodedNodeTree("DQAFAAAAAAAAQAgAAAAAAD8AAAAAAA==");
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning restore IDE0007 // Use implicit type

        // Encoded node trees can be invalid and return null
        if (nodeTree != null)
        {
            GenerateBitmap(nodeTree, "testENT");
        }
    }

#pragma warning disable IDE1006 // Naming Styles
    static void GenerateBitmap(FastNoise fastNoise, string filename, ushort size = 512)
#pragma warning restore IDE1006 // Naming Styles
    {
#pragma warning disable IDE0007 // Use implicit type
        using (BinaryWriter writer = new BinaryWriter(File.Open(filename + ".bmp", FileMode.Create)))
        {
            const uint imageDataOffset = 14u + 12u + (256u * 3u);

            // File header (14)
            writer.Write('B');
            writer.Write('M');
            writer.Write(imageDataOffset + (uint)(size * size)); // file size
            writer.Write(0); // reserved
            writer.Write(imageDataOffset); // image data offset
            // Bmp Info Header (12)
            writer.Write(12u); // size of header
            writer.Write(size); // width
            writer.Write(size); // height
            writer.Write((ushort)1); // color planes
            writer.Write((ushort)8); // bit depth
            // Colour map
            for (int i = 0; i < 256; i++)
            {
                writer.Write((byte)i);
                writer.Write((byte)i);
                writer.Write((byte)i);
            }
            // Image data
            float[] noiseData = new float[size * size];
            FastNoise.OutputMinMax minMax = fastNoise.GenUniformGrid2D(noiseData, 0, 0, size, size, 0.02f, 1337);

            float scale = 255.0f / (minMax.max - minMax.min);

            foreach (float noise in noiseData)
            {
                //Scale noise to 0 - 255
                int noiseI = (int)Math.Round((noise - minMax.min) * scale);

                writer.Write((byte)Math.Clamp(noiseI, 0, 255));
            }
        }
#pragma warning restore IDE0007 // Use implicit type
        Console.WriteLine("Created " + filename + ".bmp " + size + "x" + size);
    }
}
