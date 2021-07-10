// ref: RealSense SDK 2.45.0 RsPointCloudRenderer.cs
// Modified by Jaeu Jeong (wodndb@gmail.com)

using System;
using System.Linq;
using Intel.RealSense;
using UnityEngine;

public class RealSensePointFrameStreamer : MonoBehaviour
{
    public RsFrameProvider Source;
    public RealSenseManager rsMan;

    FrameQueue q;

    void Start()
    {
        Source.OnStart += OnStartStreaming;
        Source.OnStop += Dispose;
    }

    private void OnStartStreaming(PipelineProfile obj)
    {
        q = new FrameQueue(1);

        using (var depth = obj.Streams.FirstOrDefault(s => s.Stream == Stream.Depth && s.Format == Format.Z16)
            .As<VideoStreamProfile>())
        {
            // you can get intrinsic and extrinsic of depth frame.
        }

        Source.OnNewSample += OnNewSample;
    }

    void OnDestroy()
    {
        if (q != null)
        {
            q.Dispose();
            q = null;
        }
    }

    private void Dispose()
    {
        Source.OnNewSample -= OnNewSample;

        if (q != null)
        {
            q.Dispose();
            q = null;
        }
    }

    private void OnNewSample(Frame frame)
    {
        if (q == null)
            return;
        try
        {
            if (frame.IsComposite)
            {
                using (var fs = frame.As<FrameSet>())
                using (var points = fs.FirstOrDefault<Points>(Stream.Depth, Format.Xyz32f))
                {
                    if (points != null)
                    {
                        q.Enqueue(points);
                    }
                }
                return;
            }

            if (frame.Is(Extension.Points))
            {
                q.Enqueue(frame);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }


    protected void LateUpdate()
    {
        if (q != null)
        {
            Points points;
            if (q.PollForFrame<Points>(out points))
                using (points)
                {
                    if (points.Count != rsMan.pointBuffer.Length)
                    {
                        using (var p = points.GetProfile<VideoStreamProfile>())
                            rsMan.pointBuffer = new Vector3[p.Width * p.Height];
                    }

                    if (points.VertexData != IntPtr.Zero)
                    {
                        points.CopyVertices(rsMan.pointBuffer);
                    }
                }
        }
    }
}
