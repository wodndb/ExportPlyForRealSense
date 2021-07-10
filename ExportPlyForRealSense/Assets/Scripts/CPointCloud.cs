// Author : Jaeu Jeong (wodndb@gmail.com)

using UnityEngine;

/// <summary>
/// Custom point cloud object
/// </summary>
public class CPointCloud
{
    public Color32[] Color;
    public Vector3[] Vertices;
    
    public CPointCloud() { }

    public CPointCloud(byte[] color, Vector3[] vertices)
    {
        Color = Convert3ChannelColorByteToColor32(color);
        ArrayCopy(vertices, ref Vertices);
        
        Debug.Log(Vertices.Length);
    }
    
    public CPointCloud(Color32[] color, Vector3[] vertices)
    {
        ArrayCopy(color, ref Color);
        ArrayCopy(vertices, ref Vertices);
    }

    /// <summary>
    /// Convert 3 channel raw color data (RGB 24 bit) to Color array
    /// </summary>
    private Color32[] Convert3ChannelColorByteToColor32(byte[] rawColors)
    {
        var result = new Color32[rawColors.Length / 3];
        for (var i = 0; i < result.Length; i++)
        {
            result[i].r = rawColors[i * 3 + 0];
            result[i].g = rawColors[i * 3 + 1];
            result[i].b = rawColors[i * 3 + 2];
            result[i].a = 255;
        }

        return result;
    }
    
    /// <summary>
    /// Deep copy for value type array.
    /// </summary>
    private void ArrayCopy<T>(T[] src, ref T[] dst)
    {
        if (dst == null || dst.Length != src.Length)
        {
            dst = new T[src.Length];
        }

        for (var i = 0; i < src.Length; i++)
        {
            dst[i] = src[i];
        }
    }
}