// ref: RealSense SDK 2.45.0 RsStreamTextureRenderer.cs
// Modified by Jaeu Jeong (wodndb@gmail.com)

using System;
using System.Runtime.InteropServices;
using Intel.RealSense;
using UnityEngine;

public class RealSenseColorFrameStreamer : MonoBehaviour
{
    public RsFrameProvider Source;

    public Stream _stream;
    public Format _format;
    public int _streamIndex;

    public RealSenseManager rsMan;

    FrameQueue q;
    Predicate<Frame> matcher;

    void Start()
    {
        if (rsMan == null)
        {
            throw new Exception("RealSenseManager should not be null!");
        }
        
        Source.OnStart += OnStartStreaming;
        Source.OnStop += OnStopStreaming;
    }

    void OnDestroy()
    {
        if (q != null)
        {
            q.Dispose();
        }
    }

    protected void OnStopStreaming()
    {
        Source.OnNewSample -= OnNewSample;
        if (q != null)
        {
            q.Dispose();
            q = null;
        }
    }

    public void OnStartStreaming(PipelineProfile activeProfile)
    {
        q = new FrameQueue(1);
        matcher = new Predicate<Frame>(Matches);
        Source.OnNewSample += OnNewSample;
    }

    private bool Matches(Frame f)
    {
        using (var p = f.Profile)
            return p.Stream == _stream && p.Format == _format && (p.Index == _streamIndex || _streamIndex == -1);
    }

    void OnNewSample(Frame frame)
    {
        try
        {
            if (frame.IsComposite)
            {
                using (var fs = frame.As<FrameSet>())
                using (var f = fs.FirstOrDefault(matcher))
                {
                    if (f != null)
                        q.Enqueue(f);
                    return;
                }
            }

            if (!matcher(frame))
                return;

            using (frame)
            {
                q.Enqueue(frame);
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            // throw;
        }
    }

    bool HasBufferConflict(VideoFrame vf)
    {
        return rsMan.colorBuffer.Length != (vf.Width * vf.Height);
    }

    protected void LateUpdate()
    {
        if (q != null)
        {
            VideoFrame frame;
            if (q.PollForFrame<VideoFrame>(out frame))
                using (frame)
                    ProcessFrame(frame);
        }
    }

    private void ProcessFrame(VideoFrame frame)
    {
        if (HasBufferConflict(frame))
        {
            rsMan.colorBuffer = new byte[frame.Stride * frame.Height];
        }
        Marshal.Copy(frame.Data, rsMan.colorBuffer, 0, frame.Stride * frame.Height);
    }
}
