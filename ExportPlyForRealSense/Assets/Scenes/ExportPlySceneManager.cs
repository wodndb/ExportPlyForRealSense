// Author : Jaeu Jeong (wodndb@gmail.com)

using UnityEngine;

namespace Scenes
{
    /// <summary>
    /// Manager for ExportPlyScene
    /// </summary>
    public class ExportPlySceneManager : MonoBehaviour
    {
        public RealSenseManager rsMan;
        public string fileName; // fileName of ply file

        public void OnClickCaptureButton()
        {
            var capturedBuffer = rsMan.Capture();
            rsMan.TurnOffRsDevice();
        
            var plyExporter = new PlyExporter();
            plyExporter.ExportFromCPointCloud(capturedBuffer, fileName);
        }
    }
}
