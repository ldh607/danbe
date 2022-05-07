using System;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class WaterplayBuilder : IGraphBuilder<DetectionGraph>
    {
        public WaterplayBuilder()
        {

        }

        public DetectionGraph Build()
        {
            var bytesToMat = new BytesToMatNode();
            var matToCalibMat = new MatToCalibMatNode();
            var calibMatToWarpMat = new MatToWarpMatNode();
            var warpMatToWaterplayMat = new MatToWaterplayMatNode();
            var waterplayMatToImageContours = new MatToImageContoursNode();
            var imageContoursToViewportContours = new ImageContoursToViewportContoursNode();

            return new DetectionGraph(bytesToMat)
                .AddNode(bytesToMat, matToCalibMat)
                .AddDepth()
                .AddNode(matToCalibMat, calibMatToWarpMat)
                .AddDepth()
                .AddNode(calibMatToWarpMat, warpMatToWaterplayMat)
                .AddDepth()
                .AddNode(warpMatToWaterplayMat, waterplayMatToImageContours)
                .AddDepth()
                .AddNode(waterplayMatToImageContours, imageContoursToViewportContours);
        }
    }
}