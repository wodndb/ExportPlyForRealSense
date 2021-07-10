// Author : Jaeu Jeong (wodndb@gmail.com)

using UnityEngine;

/// <summary>
/// Manager to control realsense
/// </summary>
public class RealSenseManager : MonoBehaviour
{
    public RsDevice rsDevice;

    [HideInInspector]
    public byte[] colorBuffer;
    
    [HideInInspector]
    public Vector3[] pointBuffer;

    public void TurnOnRsDevice()
    {
        rsDevice.enabled = true;
    }

    public void TurnOffRsDevice()
    {
        rsDevice.enabled = false;
    }

    public CPointCloud Capture()
    {
        return new CPointCloud(colorBuffer, pointBuffer);
    }
}
