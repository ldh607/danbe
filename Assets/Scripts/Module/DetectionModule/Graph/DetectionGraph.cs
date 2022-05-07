using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace CellBig.Module.Detection
{
    public class DetectionGraph : IGraph
    {
        private struct Key
        {
            public int Depth;
            public Node Parent;
        }

        private class KeyComparer : IComparer<Key>
        {
            public int Compare(Key x, Key y)
            {
                return x.Depth < y.Depth ? -1 : 1;
            }
        }

        private class Node
        {
            public Node Parent;
            public IDetectionNode Value;
            public DetectionNodeOutput<object> Output;
        }

        private readonly SortedDictionary<Key, List<Node>> _graph = new SortedDictionary<Key, List<Node>>(new KeyComparer());
        private readonly List<IDetectionNode> _nodes = new List<IDetectionNode>();
        private readonly Key _rootKey = new Key();
        private readonly Node _rootNode = new Node();
        private int _depth = 0;
        private readonly ConcurrentQueue<IDetectionNode> _pushQueue = new ConcurrentQueue<IDetectionNode>();
        private readonly Queue<IDetectionNode> _pullQueue = new Queue<IDetectionNode>();
        private int _timeoutTotal;

        public DetectionGraph(IDetectionNode root)
        {
            Root = root;
            _nodes.Add(root);

            _rootKey.Depth = -1;
            _rootKey.Parent = default;
            _rootNode.Parent = null;
            _rootNode.Value = Root;
        }
 
        public DetectionGraph AddNode(IDetectionNode parent, IDetectionNode child)
        {
            // Find parent
            Node pNode = parent.Equals(Root) ? _rootNode : null;
            if (pNode == null)
            {
                foreach (var pair in _graph)
                {
                    Key pKey = pair.Key;
                    List<Node> pValue = pair.Value;

                    if (pKey.Depth == _depth - 1)
                    {
                        foreach (var pChild in pValue)
                        {
                            if (pChild.Value.Equals(parent))
                            {
                                pNode = pChild;
                                break;
                            }
                        }
                    }
                }
                if (pNode == null)
                    Debug.LogError($"[{GetType().Name}] {parent.GetType().Name} 타입의 부모 노드를 찾을 수 없습니다.");
            }

            Key key = new Key()
            {
                Depth = _depth,
                Parent = pNode 
            };
            
            if (!_graph.TryGetValue(key, out List<Node> children))
            {
                children = new List<Node>();
                _graph.Add(key, children);
            }
            
            Node node = new Node()
            {
                Parent = pNode,
                Value = child
            };
            children.Add(node);
            _nodes.Add(child);
            
            return this;
        }

        public DetectionGraph AddDepth()
        {
            _depth++;
            return this;
        }
        
        public void Run(object input, int deltaInterval)
        {
            _timeoutTotal += deltaInterval;

            DetectionNodeOutput<object> output = Root.Run(input, deltaInterval);
            _rootNode.Output = output;
            if (output.Sendable)
            {
                _pushQueue.Enqueue(Root);
                //if (IsTimeout)
                //{
                //    if (_pushQueue.TryDequeue(out IDetectionNode node))
                //        node.Reset();
                //}
            }

            foreach (var pair in _graph)
            {
                Key key = pair.Key;
                List<Node> children = pair.Value;

                Node parent = key.Parent;
                if (parent.Output.Success)
                {
                    foreach (var child in children)
                    {
                        object parentOutputVal = parent.Output.Value;
                        output = child.Value.Run(parentOutputVal, deltaInterval);
                        child.Output = output;
                        if (output.Sendable)
                        {
                            _pushQueue.Enqueue(child.Value);
                            //if (IsTimeout)
                            //{
                            //    if (_pushQueue.TryDequeue(out IDetectionNode node))
                            //        node.Reset();
                            //}
                        }
                    }
                }
                else
                {
                    foreach (var child in children)
                        child.Output = new DetectionNodeOutput<object>(null, false, false);
                }
            }
        }

        public void Update()
        {
            while (!_pushQueue.IsEmpty)
            {
                if (_pushQueue.TryDequeue(out IDetectionNode node))
                {
                    _pullQueue.Enqueue(node);
                }
            }

            foreach (var node in _pullQueue)
            {
                node.SendOutput();
            }
            _pullQueue.Clear();

            _timeoutTotal = 0;
        }

        public void Reset()
        {
            while (!_pushQueue.IsEmpty)
            {
                if (_pushQueue.TryDequeue(out IDetectionNode node))
                    node.Reset();
            }
            while (_pullQueue.Count > 0)
                _pullQueue.Dequeue().Reset();
            _pullQueue.Clear();
            _timeoutTotal = 0;
        }

        private bool IsTimeout => Timeout > 0 && _timeoutTotal >= Timeout;

        public IDetectionNode Root { get; }
        public int Timeout { get; set; }
    }
}