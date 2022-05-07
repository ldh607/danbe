using System;

namespace CellBig.Module.Detection
{
    [Serializable]
    public struct NormalizedPoint
    {
        public double X;
        public double Y;

        public NormalizedPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }
    }

    [Serializable]
    public struct NormalizedRect
    {
        public NormalizedPoint LeftTop;
        public NormalizedPoint RightTop;
        public NormalizedPoint RightBottom;
        public NormalizedPoint LeftBottom;

        public override string ToString()
        {
            return $"LeftTop : {LeftTop}\n" +
                $"RightTop : {RightTop}\n" +
                $"RightBottom : {RightBottom}\n" +
                $"LeftBottom : {LeftBottom}";
        }
    }

}