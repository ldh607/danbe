using System;
using UnityEngine;

namespace CellBig.Module.Detection
{
    public interface IGraphBuilder<TGraph> where TGraph : IGraph
    {
        TGraph Build();
    }
}