using HVT.Utility;
using HVT.VTM.Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HVT.VTM.Program
{
    public partial class Program
    {

        public event EventHandler Camstarted;
        public event EventHandler Camstopped;


        public CameraStreaming cameraStreaming;

        public void CameraInit(
            System.Windows.Controls.Image cameraHolder,
            System.Windows.Controls.Image cameraCrop)
        { 
            SetCamera(cameraHolder, cameraCrop);
        }

        private async void SetCamera(System.Windows.Controls.Image cameraHolder, System.Windows.Controls.Image cameraCrop)
        {
            Debug.Write("Camera initting....", Debug.ContentType.Notify);
            List<CameraDevice> cameras = new List<CameraDevice>();
            try
            {
                cameras = CameraDevicesEnumerator.GetAllConnectedCameras();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                Debug.Write("No camare found: " + err.Message, Debug.ContentType.Error);
                return;
            }
            if (cameras.Count < 1)
            {
                Debug.Write("No camare found.", Debug.ContentType.Error);
                return;
            }
            else
            {
                if (cameras[0].Name.ToLower().Contains("virtual"))
                {
                    Debug.Write("No camare found.", Debug.ContentType.Error);
                    return;
                }
            }

            var selectedCameraDeviceId = cameras[0].OpenCvId;
            if (cameraStreaming == null || cameraStreaming.CameraDeviceId != selectedCameraDeviceId)
            {
                Debug.Write("Apply setting...", Debug.ContentType.Notify);
                cameraStreaming?.Dispose();
                cameraStreaming = new CameraStreaming(
                    imageControlForRendering: cameraHolder,
                    imageControlForCrop: cameraCrop,
                    frameWidth: 1920,
                    frameHeight: 1080,
                    cameraDeviceId: cameras[0].OpenCvId);
                cameraStreaming.ImageUpdate += CameraStreaming_ImageUpdate;
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
