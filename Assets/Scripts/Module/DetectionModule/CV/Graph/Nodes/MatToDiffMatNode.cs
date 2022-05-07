using System;
using UnityEngine;
using OpenCVForUnity;

namespace CellBig.Module.Detection.CV
{
    public class MatToDiffMatNode : CVNodeBase<Output.DiffMat, Mat, Mat>
    {
        private readonly Mat _prevMat;
        private readonly Mat _diffMat;
        private int _intervalTotal;

        public MatToDiffMatNode()
        {
            Vector2Int resolution = Settings.OutputResolution;
            _prevMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
            _diffMat = new Mat(resolution.y, resolution.x, CvType.CV_8UC3);
        }

        ~MatToDiffMatNode()
        {
            _prevMat.Dispose();
            _diffMat.Dispose();
        }

        protected override Mat RunImpl(Mat input, int deltaInterval)
        {
            if (!Settings.UseDiff)
                return input;

            Mat diffTarget = Settings.DiffTarget == null ? _prevMat : Settings.DiffTarget;
            if (Settings.DiffMethod == DiffMethod.AbsDiff)
            {
                Core.absdiff(input, diffTarget, _diffMat);
            }
            else if (Settings.DiffMethod == DiffMethod.Subtract)
            {
                if (Settings.DiffOrder == DiffOrder.SubtractFromCurrentFrame)
                    Core.subtract(input, diffTarget, _diffMat);
                else if (Settings.DiffOrder == DiffOrder.SubtractFromPreviousFrame)
                    Core.subtract(diffTarget, input, _diffMat);
            }

            if (Settings.DiffTarget == null)
            {
                _intervalTotal += deltaInterval;
                if (_intervalTotal >= Settings.DiffInterval)
                {
                    input.copyTo(_prevMat);
                    _intervalTotal -= Settings.DiffInterval;
                }
            }

            return _diffMat;
        }

        protected override void Copy(Mat from, ref Mat to)
        {
            if (to == null)
                to = from.clone();
            else
                from.copyTo(to);
        }
    }
}