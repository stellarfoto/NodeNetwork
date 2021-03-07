﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Shared.RegexParsers
{
    public class NodeTreeBuilder
    {
        private readonly NodeTree tree;
        private readonly Node endNode;
        private readonly List<int> columnHeights = new List<int>();
        private const int spacingX = 250;
        private const int spacingY =20;
        private static readonly Vector2 outputPos = new Vector2(1000, 300);

        public NodeTreeBuilder(Node endNode)
        {
            this.endNode = endNode;
            tree = new NodeTree();
        }

        public NodeTree Build()
        {
            FillColumns(endNode);
            return tree;
        }

        private void FillColumns(Node endNode)
        {
            //var outputPos = new Vector2L(1200, 300);
            //var output = new OutputNode();// { Pos = outputPos };
            //output.PreviousNode = endNode;
            tree.AddNode(endNode);
            int startHeight = (int)outputPos.y;
            AddToColumn(endNode, startHeight, 0);
            AddNodeChildren(endNode, startHeight, 1);
        }

        private static List<Node> GetChildren(Node node)
        {
            var inputs = node.GetAllInputs().OfType<InputProcedural>();
            var children = inputs.Select(input => input.ConnectedNode).OfType<Node>().ToList();
            return children;
        }

        private void AddNodeChildren(Node parent, int pos, int column)
        {
            var children = GetChildren(parent);
            int childrenHeight = children.Skip(1).Select(node => node.GetHeight() + spacingY).Sum();
            int startPos = pos - (childrenHeight / 2);
            foreach (var child in children)
            {
                tree.AddNode(child);
                var childPos = AddToColumn(child, startPos, column);
                AddNodeChildren(child, childPos, column + 1);
            }
        }

        private int AddToColumn (Node node, int pos, int column)
        {
            if(columnHeights.Count <= column)
            {
                //Assumes that no columns are skipped
                columnHeights.Add(int.MinValue);
            }

            if (columnHeights[column] < pos)
            {
                //Leave empty spaces when appropriate
                columnHeights[column] = pos;
            }

            var xPos = outputPos.x - (column * spacingX);
            var yPos = columnHeights[column];
            columnHeights[column] += node.GetHeight() + spacingY;

            node.Pos = new Vector2(xPos, yPos);
            return yPos;
        }
    }
}
