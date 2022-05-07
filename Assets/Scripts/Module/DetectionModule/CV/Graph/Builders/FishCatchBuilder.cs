using System;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class FishCatchBuilder : IGraphBuilder<DetectionGraph>
    {
        public FishCatchBuilder()
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
            var imageContoursToImageCircles = new ImageContoursToImageCirclesNode();
            var imageContoursToImageQuads = new ImageContoursToImageQuadsNode();
            var imageCirclesToViewportCircles = new ImageCirclesToViewportCirclesNode();
            var imageQuadsToViewportQuads = new ImageQuadsToViewportQuadsNode();
            
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
                .AddNode(binaryMatToImageContours, imageContoursToImageCircles)
                .AddNode(binaryMatToImageContours, imageContoursToImageQuads)
                .AddDepth()
                .AddNode(imageContoursToImageCircles, imageCirclesToViewportCircles)
                .AddNode(imageContoursToImageQuads, imageQuadsToViewportQuads);
        }
    }
}