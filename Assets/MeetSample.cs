/*
*   Meet
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using UnityEngine;
    using UnityEngine.UI;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Vision;

    public class MeetSample : MonoBehaviour {

        [Header(@"UI")]
        public RawImage rawImage;
        public AspectRatioFitter aspectFitter;

        private CameraDevice cameraDevice;
        private TextureOutput cameraTextureOutput;
        private RenderTexture matteImage;

        private MLModel model;
        private MeetPredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get the default camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraDevice.previewResolution = (1280, 720);
            cameraTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(cameraTextureOutput);
            // Create matte texture
            var cameraTexture = await cameraTextureOutput;
            matteImage = new RenderTexture(cameraTexture.width, cameraTexture.height, 0);
            // Display matte texture on UI
            rawImage.texture = matteImage;
            aspectFitter.aspectRatio = (float)cameraTexture.width / cameraTexture.height;            
            // Create the Meet predictor
            Debug.Log("Fetching model from NatML...");
            var modelData = await MLModelData.FromHub("@natml/meet");
            model = modelData.Deserialize();
            predictor = new MeetPredictor(model);
        }

        void Update () {
            // Check that the predictor has been created
            if (predictor == null)
                return;
            // Predict
            var matte = predictor.Predict(cameraTextureOutput.texture);
            matte.Render(matteImage);
        }

        void OnDisable () {
            // Dispose model
            model?.Dispose();
        }
    }
}