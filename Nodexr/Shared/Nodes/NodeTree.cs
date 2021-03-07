﻿using Nodexr.Shared.NodeTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.Nodes
{
    public class NodeTree
    {
        public event EventHandler OutputChanged;

        private readonly List<INode> nodes = new List<INode>();
        public IEnumerable<INode> Nodes => nodes.AsReadOnly();

        public NodeResult CachedOutput { get; private set; }

        public void AddNode(INode node)
        {
            nodes.Add(node);

            if (node is OutputNode)
            {
                node.OutputChanged += OnOutputChanged;
                RecalculateOutput();
            }
        }

        public void DeleteNode(INode nodeToRemove)
        {
            DeleteOutputNoodles(nodeToRemove);
            nodes.Remove(nodeToRemove);
            foreach(var input in nodeToRemove.GetAllInputs()
                .OfType<InputProcedural>()
                .Where(input => input.Connected))
            {
                input.ConnectedNode = null;
            }
            RecalculateOutput();
        }

        private void OnOutputChanged(object sender, EventArgs e)
        {
            RecalculateOutput();
        }

        private OutputNode GetOutputNode()
        {
            return nodes.OfType<OutputNode>().Single();
        }

        public void RecalculateOutput()
        {
            var outputNode = GetOutputNode();

            CachedOutput = outputNode.CachedOutput;
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteOutputNoodles(INode nodeToRemove)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.GetAllInputs().OfType<InputProcedural>())
                {
                    DeleteNoodlesBetween(nodeToRemove, input);
                }
            }
        }

        private void DeleteNoodlesBetween(INode node, InputProcedural input)
        {
            if (input.ConnectedNode == node)
            {
                input.ConnectedNode = null;
            }
        }
    }
}
