using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

using HVT.VTM.Directorys;
using HVT.VTM.AppSetting;
using HVT.Devices;
using HVT.Utility;

namespace HVT.VTM.Program
{
    public partial class Program
    {

        public event EventHandler Camstarted;
        public event EventHandler Camstopped;

        public CameraStreaming cameraStreaming;
        public void CameraInit(System.Windows.Controls.Image cameraHolder, System.Windows.Controls.Image cameraCrop)
        {
            SetCamera(cameraHolder, cameraCrop);
        }

        private async void SetCamera(System.Windows.Controls.Image cameraHolder, System.Windows.Controls.Image cameraCrop)
        {
            Debug.Write("Camera initting....", Debug.ContentType.Notify);
            var cameras = CameraDevicesEnumerator.GetAllConnectedCameras();
            var selectedCameraDeviceId = cameras[0].OpenCvId;
            if (cameraStreaming == null || cameraStreaming.CameraDeviceId != selectedCameraDeviceId)
            {
                Debug.Write("Apply setting...", Debug.ContentType.Notify);
                cameraStreaming?.Dispose();
                cameraStreaming = new CameraStreaming(
                    imageControlForRendering: cameraHolder,
                    imageControlForCrop: cameraCrop,
                    frameWidth: 1280,
                    frameHeight: 1720,
                    cameraDeviceId: cameras[0].OpenCvId);
            }

            try
            {
                await cameraStreaming.Start();
                Debug.Write("Camera start", Debug.ContentType.Notify);
                Camstarted?.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Debug.Write("Camera start get error: " + ex.Message, Debug.ContentType.Error);
            }

        }

        public void CameraDisponse()
        {
            Camstopped?.Invoke(null, null);
        }
    }
}
