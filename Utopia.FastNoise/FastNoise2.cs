// This file is a part of the project Utopia(Or is a part of its subproject).
// Copyright 2020-2023 mingmoe(http://kawayi.moe)
// The file was licensed under the AGPL 3.0-or-later license

using System.Runtime.InteropServices;

namespace Utopia.FastNoise;

public class FastNoise
{
    public struct OutputMinMax
    {
        public OutputMinMax(float minValue = float.PositiveInfinity, float maxValue = float.NegativeInfinity)
        {
            min = minValue;
            max = maxValue;
        }

        public OutputMinMax(float[] nativeOutputMinMax)
        {
            min = nativeOutputMinMax[0];
            max = nativeOutputMinMax[1];
        }

        public void Merge(OutputMinMax other)
        {
            min = Math.Min(min, other.min);
            max = Math.Max(max, other.max);
        }

        public float min;
        public float max;
    }

    public FastNoise(string metadataName)
    {
        if (!metadataNameLookup.TryGetValue(FormatLookup(metadataName), out mMetadataId))
        {
            throw new ArgumentException("Failed to find metadata name: " + metadataName);
        }

        mNodeHandle = fnNewFromMetadata(mMetadataId);
    }

    private FastNoise(IntPtr nodeHandle)
    {
        mNodeHandle = nodeHandle;
        mMetadataId = fnGetMetadataID(nodeHandle);
    }

    ~FastNoise()
    {
        fnDeleteNodeRef(mNodeHandle);
    }

    public static FastNoise? FromEncodedNodeTree(string encodedNodeTree)
    {
        IntPtr nodeHandle = fnNewFromEncodedNodeTree(encodedNodeTree);

        return nodeHandle == IntPtr.Zero ? null : new FastNoise(nodeHandle);
    }

    public uint GetSIMDLevel() => fnGetSIMDLevel(mNodeHandle);

    public void Set(string memberName, float value)
    {
#pragma warning disable IDE0018 // Inline variable declaration
        Metadata.Member member;
#pragma warning restore IDE0018 // Inline variable declaration
        if (!nodeMetadata[mMetadataId].members.TryGetValue(FormatLookup(memberName), out member))
        {
            throw new ArgumentException("Failed to find member name: " + memberName);
        }

        switch (member.type)
        {
            case Metadata.Member.Type.Float:
                if (!fnSetVariableFloat(mNodeHandle, member.index, value))
                {
                    throw new ExternalException("Failed to set float value");
                }
                break;

            case Metadata.Member.Type.Hybrid:
                if (!fnSetHybridFloat(mNodeHandle, member.index, value))
                {
                    throw new ExternalException("Failed to set float value");
                }
                break;

            default:
                throw new ArgumentException(memberName + " cannot be set to a float value");
        }
    }

    public void Set(string memberName, int value)
    {
#pragma warning disable IDE0018 // Inline variable declaration
        Metadata.Member member;
#pragma warning restore IDE0018 // Inline variable declaration
        if (!nodeMetadata[mMetadataId].members.TryGetValue(FormatLookup(memberName), out member))
        {
            throw new ArgumentException("Failed to find member name: " + memberName);
        }

        if (member.type != Metadata.Member.Type.Int)
        {
            throw new ArgumentException(memberName + " cannot be set to an int value");
        }

        if (!fnSetVariableIntEnum(mNodeHandle, member.index, value))
        {
            throw new ExternalException("Failed to set int value");
        }
    }

    public void Set(string memberName, string enumValue)
    {
#pragma warning disable IDE0018 // Inline variable declaration
        Metadata.Member member;
#pragma warning restore IDE0018 // Inline variable declaration
        if (!nodeMetadata[mMetadataId].members.TryGetValue(FormatLookup(memberName), out member))
        {
            throw new ArgumentException("Failed to find member name: " + memberName);
        }

        if (member.type != Metadata.Member.Type.Enum)
        {
            throw new ArgumentException(memberName + " cannot be set to an enum value");
        }

#pragma warning disable IDE0018 // Inline variable declaration
        int enumIdx;
#pragma warning restore IDE0018 // Inline variable declaration
        if (!member.enumNames.TryGetValue(FormatLookup(enumValue), out enumIdx))
        {
            throw new ArgumentException("Failed to find enum value: " + enumValue);
        }

        if (!fnSetVariableIntEnum(mNodeHandle, member.index, enumIdx))
        {
            throw new ExternalException("Failed to set enum value");
        }
    }

    public void Set(string memberName, FastNoise nodeLookup)
    {
#pragma warning disable IDE0018 // Inline variable declaration
        Metadata.Member member;
#pragma warning restore IDE0018 // Inline variable declaration
        if (!nodeMetadata[mMetadataId].members.TryGetValue(FormatLookup(memberName), out member))
        {
            throw new ArgumentException("Failed to find member name: " + memberName);
        }

        switch (member.type)
        {
            case Metadata.Member.Type.NodeLookup:
                if (!fnSetNodeLookup(mNodeHandle, member.index, nodeLookup.mNodeHandle))
                {
                    throw new ExternalException("Failed to set node lookup");
                }
                break;

            case Metadata.Member.Type.Hybrid:
                if (!fnSetHybridNodeLookup(mNodeHandle, member.index, nodeLookup.mNodeHandle))
                {
                    throw new ExternalException("Failed to set node lookup");
                }
                break;

            default:
                throw new ArgumentException(memberName + " cannot be set to a node lookup");
        }
    }

    public OutputMinMax GenUniformGrid2D(float[] noiseOut,
                                   int xStart, int yStart,
                                   int xSize, int ySize,
                                   float frequency, int seed)
    {
        float[] minMax = new float[2];
        _ = fnGenUniformGrid2D(mNodeHandle, noiseOut, xStart, yStart, xSize, ySize, frequency, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenUniformGrid3D(float[] noiseOut,
                                   int xStart, int yStart, int zStart,
                                   int xSize, int ySize, int zSize,
                                   float frequency, int seed)
    {
        float[] minMax = new float[2];
        _ = fnGenUniformGrid3D(mNodeHandle, noiseOut, xStart, yStart, zStart, xSize, ySize, zSize, frequency, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenUniformGrid4D(float[] noiseOut,
                                   int xStart, int yStart, int zStart, int wStart,
                                   int xSize, int ySize, int zSize, int wSize,
                                   float frequency, int seed)
    {
        float[] minMax = new float[2];
        _ = fnGenUniformGrid4D(mNodeHandle, noiseOut, xStart, yStart, zStart, wStart, xSize, ySize, zSize, wSize, frequency, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenTileable2D(float[] noiseOut,
                                   int xSize, int ySize,
                                   float frequency, int seed)
    {
        float[] minMax = new float[2];
        fnGenTileable2D(mNodeHandle, noiseOut, xSize, ySize, frequency, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenPositionArray2D(float[] noiseOut,
                                         float[] xPosArray, float[] yPosArray,
                                         float xOffset, float yOffset,
                                         int seed)
    {
        float[] minMax = new float[2];
        fnGenPositionArray2D(mNodeHandle, noiseOut, xPosArray.Length, xPosArray, yPosArray, xOffset, yOffset, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenPositionArray3D(float[] noiseOut,
                                         float[] xPosArray, float[] yPosArray, float[] zPosArray,
                                         float xOffset, float yOffset, float zOffset,
                                         int seed)
    {
        float[] minMax = new float[2];
        fnGenPositionArray3D(mNodeHandle, noiseOut, xPosArray.Length, xPosArray, yPosArray, zPosArray, xOffset, yOffset, zOffset, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public OutputMinMax GenPositionArray4D(float[] noiseOut,
                                         float[] xPosArray, float[] yPosArray, float[] zPosArray, float[] wPosArray,
                                         float xOffset, float yOffset, float zOffset, float wOffset,
                                         int seed)
    {
        float[] minMax = new float[2];
        fnGenPositionArray4D(mNodeHandle, noiseOut, xPosArray.Length, xPosArray, yPosArray, zPosArray, wPosArray, xOffset, yOffset, zOffset, wOffset, seed, minMax);
        return new OutputMinMax(minMax);
    }

    public float GenSingle2D(float x, float y, int seed) => fnGenSingle2D(mNodeHandle, x, y, seed);

    public float GenSingle3D(float x, float y, float z, int seed) => fnGenSingle3D(mNodeHandle, x, y, z, seed);

    public float GenSingle4D(float x, float y, float z, float w, int seed) => fnGenSingle4D(mNodeHandle, x, y, z, w, seed);

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE1006 // Naming Styles
    private IntPtr mNodeHandle = IntPtr.Zero;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE1006 // Naming Styles
    private int mMetadataId = -1;
#pragma warning restore IDE1006 // Naming Styles
#pragma warning restore IDE0044 // Add readonly modifier
    public class Metadata
    {
        public struct Member
        {
            public enum Type
            {
                Float,
                Int,
                Enum,
                NodeLookup,
                Hybrid,
            }

            public string name;
            public Type type;
            public int index;
            public Dictionary<string, int> enumNames;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            public Member()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            {

            }
        }

        public int id;
        public string name = string.Empty;
        public Dictionary<string, Member> members = new();
    }

    static FastNoise()
    {
        int metadataCount = fnGetMetadataCount();

        nodeMetadata = new Metadata[metadataCount];
        metadataNameLookup = new Dictionary<string, int>(metadataCount);

        // Collect metadata for all FastNoise node classes
        for (int id = 0; id < metadataCount; id++)
        {
#pragma warning disable IDE0017 // Simplify object initialization
            Metadata metadata = new();
#pragma warning restore IDE0017 // Simplify object initialization

            metadata.id = id;
#pragma warning disable CS8604 // Possible null reference argument.
            metadata.name = FormatLookup(Marshal.PtrToStringAnsi(fnGetMetadataName(id)));
#pragma warning restore CS8604 // Possible null reference argument.
            //Console.WriteLine(id + " - " + metadata.name);
            metadataNameLookup.Add(metadata.name, id);

            int variableCount = fnGetMetadataVariableCount(id);
            int nodeLookupCount = fnGetMetadataNodeLookupCount(id);
            int hybridCount = fnGetMetadataHybridCount(id);
            metadata.members = new Dictionary<string, Metadata.Member>(variableCount + nodeLookupCount + hybridCount);

            // Init variables
            for (int variableIdx = 0; variableIdx < variableCount; variableIdx++)
            {
#pragma warning disable IDE0017 // Simplify object initialization
                Metadata.Member member = new();
#pragma warning restore IDE0017 // Simplify object initialization

#pragma warning disable CS8604 // Possible null reference argument.
                member.name = FormatLookup(Marshal.PtrToStringAnsi(fnGetMetadataVariableName(id, variableIdx)));
#pragma warning restore CS8604 // Possible null reference argument.
                member.type = (Metadata.Member.Type)fnGetMetadataVariableType(id, variableIdx);
                member.index = variableIdx;

                member.name = FormatDimensionMember(member.name, fnGetMetadataVariableDimensionIdx(id, variableIdx));

                // Get enum names
                if (member.type == Metadata.Member.Type.Enum)
                {
                    int enumCount = fnGetMetadataEnumCount(id, variableIdx);
                    member.enumNames = new Dictionary<string, int>(enumCount);

                    for (int enumIdx = 0; enumIdx < enumCount; enumIdx++)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        member.enumNames.Add(FormatLookup(Marshal.PtrToStringAnsi(fnGetMetadataEnumName(id, variableIdx, enumIdx))), enumIdx);
#pragma warning restore CS8604 // Possible null reference argument.
                    }
                }

                metadata.members.Add(member.name, member);
            }

            // Init node lookups
            for (int nodeLookupIdx = 0; nodeLookupIdx < nodeLookupCount; nodeLookupIdx++)
            {
#pragma warning disable IDE0017 // Simplify object initialization
                Metadata.Member member = new();
#pragma warning restore IDE0017 // Simplify object initialization

#pragma warning disable CS8604 // Possible null reference argument.
                member.name = FormatLookup(Marshal.PtrToStringAnsi(fnGetMetadataNodeLookupName(id, nodeLookupIdx)));
#pragma warning restore CS8604 // Possible null reference argument.
                member.type = Metadata.Member.Type.NodeLookup;
                member.index = nodeLookupIdx;

                member.name = FormatDimensionMember(member.name, fnGetMetadataNodeLookupDimensionIdx(id, nodeLookupIdx));

                metadata.members.Add(member.name, member);

            }

            // Init hybrids
            for (int hybridIdx = 0; hybridIdx < hybridCount; hybridIdx++)
            {
#pragma warning disable IDE0017 // Simplify object initialization
                Metadata.Member member = new();
#pragma warning restore IDE0017 // Simplify object initialization

#pragma warning disable CS8604 // Possible null reference argument.
                member.name = FormatLookup(Marshal.PtrToStringAnsi(fnGetMetadataHybridName(id, hybridIdx)));
#pragma warning restore CS8604 // Possible null reference argument.
                member.type = Metadata.Member.Type.Hybrid;
                member.index = hybridIdx;

                member.name = FormatDimensionMember(member.name, fnGetMetadataHybridDimensionIdx(id, hybridIdx));

                metadata.members.Add(member.name, member);

            }
            nodeMetadata[id] = metadata;
        }
    }

    // Append dimension char where neccessary 
    private static string FormatDimensionMember(string name, int dimIdx)
    {
        if (dimIdx >= 0)
        {
            char[] dimSuffix = new char[] { 'x', 'y', 'z', 'w' };
            name += dimSuffix[dimIdx];
        }
        return name;
    }

    // Ignores spaces and caps, harder to mistype strings
    private static string FormatLookup(string s) => s.Replace(" ", "").ToLower();

#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0044 // Add readonly modifier
    private static Dictionary<string, int> metadataNameLookup;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore IDE1006 // Naming Styles
#pragma warning disable IDE1006 // Naming Styles
#pragma warning disable IDE0044 // Add readonly modifier
    private static Metadata[] nodeMetadata;
#pragma warning restore IDE0044 // Add readonly modifier
#pragma warning restore IDE1006 // Naming Styles

    private const string NATIVE_LIB = "FastNoise";

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnNewFromMetadata(int id, uint simdLevel = 0);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable CA2101 // Specify marshaling for P/Invoke string arguments
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnNewFromEncodedNodeTree([MarshalAs(UnmanagedType.LPStr)] string encodedNodeTree, uint simdLevel = 0);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
#pragma warning restore CA2101 // Specify marshaling for P/Invoke string arguments

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern void fnDeleteNodeRef(IntPtr nodeHandle);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern uint fnGetSIMDLevel(IntPtr nodeHandle);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataID(IntPtr nodeHandle);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern uint fnGenUniformGrid2D(IntPtr nodeHandle, float[] noiseOut,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                   int xStart, int yStart,
                                   int xSize, int ySize,
                                   float frequency, int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern uint fnGenUniformGrid3D(IntPtr nodeHandle, float[] noiseOut,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                   int xStart, int yStart, int zStart,
                                   int xSize, int ySize, int zSize,
                                   float frequency, int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern uint fnGenUniformGrid4D(IntPtr nodeHandle, float[] noiseOut,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                   int xStart, int yStart, int zStart, int wStart,
                                   int xSize, int ySize, int zSize, int wSize,
                                   float frequency, int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern void fnGenTileable2D(IntPtr node, float[] noiseOut,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                    int xSize, int ySize,
                                    float frequency, int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern void fnGenPositionArray2D(IntPtr node, float[] noiseOut, int count,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                         float[] xPosArray, float[] yPosArray,
                                         float xOffset, float yOffset,
                                         int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern void fnGenPositionArray3D(IntPtr node, float[] noiseOut, int count,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                         float[] xPosArray, float[] yPosArray, float[] zPosArray,
                                         float xOffset, float yOffset, float zOffset,
                                         int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern void fnGenPositionArray4D(IntPtr node, float[] noiseOut, int count,
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
                                         float[] xPosArray, float[] yPosArray, float[] zPosArray, float[] wPosArray,
                                         float xOffset, float yOffset, float zOffset, float wOffset,
                                         int seed, float[] outputMinMax);

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern float fnGenSingle2D(IntPtr node, float x, float y, int seed);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern float fnGenSingle3D(IntPtr node, float x, float y, float z, int seed);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern float fnGenSingle4D(IntPtr node, float x, float y, float z, float w, int seed);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataCount();
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnGetMetadataName(int id);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    // Variable
    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataVariableCount(int id);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnGetMetadataVariableName(int id, int variableIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataVariableType(int id, int variableIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataVariableDimensionIdx(int id, int variableIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataEnumCount(int id, int variableIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnGetMetadataEnumName(int id, int variableIndex, int enumIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern bool fnSetVariableFloat(IntPtr nodeHandle, int variableIndex, float value);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern bool fnSetVariableIntEnum(IntPtr nodeHandle, int variableIndex, int value);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    // Node Lookup
    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataNodeLookupCount(int id);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnGetMetadataNodeLookupName(int id, int nodeLookupIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataNodeLookupDimensionIdx(int id, int nodeLookupIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern bool fnSetNodeLookup(IntPtr nodeHandle, int nodeLookupIndex, IntPtr nodeLookupHandle);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    // Hybrid
    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataHybridCount(int id);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern IntPtr fnGetMetadataHybridName(int id, int nodeLookupIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern int fnGetMetadataHybridDimensionIdx(int id, int nodeLookupIndex);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern bool fnSetHybridNodeLookup(IntPtr nodeHandle, int nodeLookupIndex, IntPtr nodeLookupHandle);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time

    [DllImport(NATIVE_LIB)]
#pragma warning disable SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
    private static extern bool fnSetHybridFloat(IntPtr nodeHandle, int nodeLookupIndex, float value);
#pragma warning restore SYSLIB1054 // Use 'LibraryImportAttribute' instead of 'DllImportAttribute' to generate P/Invoke marshalling code at compile time
}
