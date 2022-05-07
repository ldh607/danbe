using System;
using UnityEngine;

namespace CellBig.Module.Detection.CV
{
    public class BallpoolBuilder : IGraphBuilder<DetectionGraph>
    {
        public BallpoolBuilder()
        {

        }

        public DetectionGraph Build()
        {
            var bytesToMat = new BytesToMatNode();
            var matToWarpMat = new MatToWarpMatNode();
            var warpMatToDepthBinaryMat = new MatToDepthBinaryMatNode();
            var depthBinaryMatToBinaryDiffMat = new BinaryMatToBinaryDiffMat();
            var binaryDiffMatToImageContours = new MatToImageContoursNode();
            var imageContoursToImageRects = new ImageContoursToImageRectsNode();
            var imageRectsToViewportRects = new ImageRectsToViewportRects();

            return new DetectionGraph(bytesToMat)
                .AddNode(bytesToMat, matToWarpMat)
                .AddDepth()
                .AddNode(matToWarpMat, warpMatToDepthBinaryMat)
                .AddDepth()
                .AddNode(warpMatToDepthBinaryMat, depthBinaryMatToBinaryDiffMat)
                .AddDepth()
                .AddNode(depthBinaryMatToBinaryDiffMat, binaryDiffMatToImageContours)
                .AddDepth()
                .AddNode(binaryDiffMatToImageContours, imageContoursToImageRects)
                .AddDepth()
                .AddNode(imageContoursToImageRects, imageRectsToViewportRects);
        }
    }
}