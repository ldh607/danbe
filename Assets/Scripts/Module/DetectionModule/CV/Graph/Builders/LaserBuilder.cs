using System;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class LaserBuilder : IGraphBuilder<DetectionGraph>
    {
        public LaserBuilder()
        {

        }

        public DetectionGraph Build()
        {
            var bytesToMat = new BytesToMatNode();
            var matToCalibMat = new MatToCalibMatNode();
            var calibMatToWarpMat = new MatToWarpMatNode();
            var warpMatToDiffMat = new MatToDiffMatNode();
            var diffMatToBinaryMat = new MatToBinaryMatNode();
            var binaryMatToImageContours = new MatToImageContoursNode();
            var imageContoursToImageRects = new ImageContoursToImageRectsNode();
            var imageRectsToViewportRects = new ImageRectsToViewportRects();

            return new DetectionGraph(bytesToMat)
                .AddNode(bytesToMat, matToCalibMat)
                .AddDepth()
                .AddNode(matToCalibMat, calibMatToWarpMat)
                .AddDepth()
                .AddNode(calibMatToWarpMat, warpMatToDiffMat)
                .AddDepth()
                .AddNode(warpMatToDiffMat, diffMatToBinaryMat)
                .AddDepth()
                .AddNode(diffMatToBinaryMat, binaryMatToImageContours)
                .AddDepth()
                .AddNode(binaryMatToImageContours, imageContoursToImageRects)
                .AddDepth()
                .AddNode(imageContoursToImageRects, imageRectsToViewportRects);
        }
    }
}