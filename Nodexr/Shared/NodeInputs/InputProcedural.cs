﻿using System;
using System.Threading.Tasks;
using Nodexr.Shared.Nodes;

namespace Nodexr.Shared.NodeInputs
{
    public class InputProcedural : NodeInput, INoodleData
    {
        private INodeOutput connectedNode;

        public INodeOutput ConnectedNode
        {
            get => connectedNode;
            set
            {
                if (connectedNode != null)
                {
                    connectedNode.OutputChanged -= OnValueChanged;
                }
                if (value != null)
                {
                    value.OutputChanged += OnValueChanged;
                }
                connectedNode = value;
                OnValueChanged();
            }
        }

        public Vector2 StartPos => connectedNode.OutputPos;

        public Vector2 EndPos => Pos;

        public bool Connected => connectedNode != null && Enabled();

        public override int Height => 32;

        public event EventHandler NoodleChanged;

        public string GetValue()
        {
            return ConnectedNode?.CachedOutput.Expression ?? "";
        }

        public bool IsConnected => connectedNode != null;

        public NodeResult Value => ConnectedNode?.CachedOutput;

        /// <summary>
        /// Causes the connected noodle to be re-rendered
        /// </summary>
        public void Refresh()
        {
            NoodleChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface INoodleData
    {
        Vector2 StartPos { get; }
        Vector2 EndPos { get; }
        bool Connected { get; }
        void Refresh();

        event EventHandler NoodleChanged;
    }
}
