using System;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class WaterplayTestBuilder : IGraphBuilder<DetectionGraph>
    {
        public WaterplayTestBuilder()
        {

        }

        public DetectionGraph Build()
        {
            var bytesToMat = new BytesToMatNode();
            var matToCalibMat = new MatToCalibMatNode();
            var calibMatToWarpMat = new MatToWarpMatNode();
            var warpMatToBinaryMat = new MatToBinaryMatNode();
            var binaryMatToImageContours = new MatToImageContoursNode();
            var imageContoursToViewportContours = new ImageContoursToViewportContoursNode();

            return new DetectionGraph(bytesToMat)
                .AddNode(bytesToMat, matToCalibMat)
                .AddDepth()
                .AddNode(matToCalibMat, calibMatToWarpMat)
                .AddDepth()
                .AddNode(calibMatToWarpMat, warpMatToBinaryMat)
                .AddDepth()
                .AddNode(warpMatToBinaryMat, binaryMatToImageContours)
                .AddDepth()
                .AddNode(binaryMatToImageContours, imageContoursToViewportContours);
        }
    }
}