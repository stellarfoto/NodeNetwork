﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Linq;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using System.Collections.Generic;

namespace Nodexr.Shared.Services
{
    public interface INodeDragService
    {
        void OnStartNodeDrag(INode nodeToDrag, MouseEventArgs e);
        void OnDrop(MouseEventArgs e);
        Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e);
        void CancelDrag();
        bool IsDrag(MouseEventArgs e);
    }

    public class NodeDragService : INodeDragService
    {
        private readonly INodeHandler nodeHandler;
        private readonly IJSRuntime jsRuntime;

        private INode nodeToDrag;
        private List<InputProcedural> nodeToDragOutputs;

        private Vector2 cursorStartPos;
        private Vector2 nodeStartPos;

        private bool isDraggingNewNode = false;

        public NodeDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime)
        {
            this.nodeHandler = nodeHandler;
            this.jsRuntime = jsRuntime;
            jsRuntime.InvokeVoidAsync("addDotNetSingletonService", "DotNetNodeDragService", DotNetObjectReference.Create(this));
        }

        public void OnStartNodeDrag(INode nodeToDrag, MouseEventArgs e)
        {
            this.nodeToDrag = nodeToDrag;

            this.nodeToDragOutputs = nodeHandler.Tree.Nodes
                .SelectMany(node => node.GetAllInputs()
                    .OfType<InputProcedural>()
                    .Where(input => input.ConnectedNode == nodeToDrag)).ToList();

            cursorStartPos = e.GetClientPos();
            nodeStartPos = nodeToDrag.Pos;
        }

        public bool IsDrag(MouseEventArgs e)
        {
            const int dragThreshold = 4; //Length in px to consider a drag (instead of a click)
            var mouseOffset = e.GetClientPos() - cursorStartPos;
            return mouseOffset.GetLength() > dragThreshold;
        }

        public async Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            this.nodeToDrag = nodeToDrag;
            isDraggingNewNode = true;
            cursorStartPos = e.GetClientPos();
            var scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY)
                .ConfigureAwait(false);
            int x = (int)scaledPos[0];
            int y = (int)scaledPos[1];

            this.nodeToDrag.Pos = new Vector2(x - 75, y - 15);
        }

        [JSInvokable]
        public void DragNode(double posX, double posY)
        {
            var dragOffset = (new Vector2(posX, posY) - cursorStartPos) / ZoomHandler.Zoom;
            nodeToDrag.Pos = nodeStartPos + dragOffset;
            nodeToDrag.OnLayoutChanged(this, EventArgs.Empty);
            foreach (var input in nodeToDrag.GetAllInputs().OfType<InputProcedural>())
            {
                input.Refresh();
            }

            foreach (var input in nodeToDragOutputs)
            {
                input.Refresh();
            }
        }

        public void OnDrop(MouseEventArgs e)
        {
            //Console.WriteLine("Dropping node");
            if (nodeToDrag != null)
            {
                //TODO: Refactor this
                if (isDraggingNewNode)
                {
                    isDraggingNewNode = false;
                    nodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                    nodeHandler.Tree.AddNode(nodeToDrag);
                }
                nodeToDrag = null;
            }
        }

        public void CancelDrag()
        {
            nodeToDrag = null;
            isDraggingNewNode = false;
        }
    }
}
