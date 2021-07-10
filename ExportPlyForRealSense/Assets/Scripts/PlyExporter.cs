// Author  : Jaeu Jeong (wodndb@gmail.com)
// Comment : I expired this codes from Keijiro's plyImporter in pcx. (https://github.com/keijiro/Pcx)

using System;
using System.IO;
using System.Text;
using UnityEngine;

/// <summary>
/// Export point cloud to ply
/// </summary>
public class PlyExporter
{
    public void ExportFromCPointCloud(CPointCloud pc, string path)
    {
        if (!WriteHeader(pc, path))
            throw new ArgumentException("pc is invalid");

        WriteBody(pc, path);
    }
    
    // if write is fail, return false. else, true.
    private bool WriteHeader(CPointCloud pc, string path)
    {
        int vertexNum = pc.Vertices.Length;
        if (vertexNum <= 0) return false;

        using (var sw = new StreamWriter(path, false, Encoding.ASCII))
        {
            sw.Write("ply\n" +
                     "format binary_little_endian 1.0\n" +
                     "element vertex " + vertexNum + "\n" +
                     "property float32 x\n" +
                     "property float32 y\n" +
                     "property float32 z\n");

            if (pc.Color != null && pc.Color.Length > 0)
            {
                sw.Write("property uchar red\n" +
                         "property uchar green\n" +
                         "property uchar blue\n");
            }

            sw.Write("end_header\n");
        }

        return true;
    }

    private void WriteBody(CPointCloud pc, string path)
    {
        var fs = File.Open(path, FileMode.Append);
        
        using (var wr = new BinaryWriter(fs))
        {
            var verticesLen = pc.Vertices.Length;
            if (verticesLen <= 0) return;

            if (pc.Color != null && pc.Color.Length > 0)
            {
                wr.Write(ConvertToBytes(pc.Vertices, pc.Color));
            }
            else
            {
                wr.Write(ConvertToBytes(pc.Vertices));
            }
        }
        
        Debug.Log("File save finished");
    }

    private byte[] ConvertToBytes(Vector3[] vertices, Color32[] colors)
    {
        var len = vertices.Length;
        var pointInfoByteSize = sizeof(float) * 3 + 3;
        var result = new byte[pointInfoByteSize * len];
        for (int i = 0; i < len; i++)
        {
            var idx = i * pointInfoByteSize;

            var x = BitConverter.GetBytes(vertices[i].x);
            result[idx + 0] = x[0];
            result[idx + 1] = x[1];
            result[idx + 2] = x[2];
            result[idx + 3] = x[3];

            var y = BitConverter.GetBytes(vertices[i].y);
            result[idx + 4] = y[0];
            result[idx + 5] = y[1];
            result[idx + 6] = y[2];
            result[idx + 7] = y[3];

            var z = BitConverter.GetBytes(vertices[i].z);
            result[idx + 8] = z[0];
            result[idx + 9] = z[1];
            result[idx + 10] = z[2];
            result[idx + 11] = z[3];

            result[idx + 12] = colors[i].r;
            result[idx + 13] = colors[i].g;
            result[idx + 14] = colors[i].b;
        }

        return result;
    }
    
    private byte[] ConvertToBytes(Vector3[] src)
    {
        var len = src.Length;
        var pointInfoByteSize = sizeof(float) * 3;
        
        var result = new byte[pointInfoByteSize * len];
        for (var i = 0; i < len; i++)
        {
            var idx = i * 12;
            
            // x
            var x = BitConverter.GetBytes(src[i].x);
            result[idx + 0] = x[0];
            result[idx + 1] = x[1];
            result[idx + 2] = x[2];
            result[idx + 3] = x[3];

            // y
            var y = BitConverter.GetBytes(src[i].y);
            result[idx + 4] = y[0];
            result[idx + 5] = y[1];
            result[idx + 6] = y[2];
            result[idx + 7] = y[3];

            // z
            var z = BitConverter.GetBytes(src[i].z);
            result[idx + 8] = z[0];
            result[idx + 9] = z[1];
            result[idx + 10] = z[2];
            result[idx + 11] = z[3];
        }

        return result;
    }
}