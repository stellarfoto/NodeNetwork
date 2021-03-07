﻿class NoodleDragHandler {
    private noodleElement: HTMLElement = null;
    private isValid: boolean = false;

    startX: number = 0;
    startY: number = 0;
    endX: number = 0;
    endY: number = 0;

    public startNoodleDrag = (startX: number, startY: number) => {
        this.noodleElement = document.getElementById("tempNoodle");
        this.startX = this.endX = startX;
        this.startY = this.endY = startY;
        window.addEventListener("dragover", this.dragNoodle);

        this.updatePath();
        this.setInvalid();
    }

    dragNoodle = (event: DragEvent) => {
        [this.endX, this.endY] = (window['panzoom'] as PanZoom).clientToGraphPos(event.clientX, event.clientY);
        this.updatePath();
    }

    endDrag = () => {
        window.removeEventListener("dragover", this.dragNoodle);
    }

    updatePath = () => {
        if (this.noodleElement != null) {
            this.setPath(
                this.startX,
                this.startY,
                this.endX,
                this.endY);
        }
    }

    setPath = (startX: number, startY: number, endX: number, endY: number) => {
        let path = NoodleDragHandler.getNoodlePath(startX, startY, endX, endY);
        if (this.noodleElement != null) {
            this.noodleElement.setAttribute("d", path);
        }
    }

    setValid = () => {
        if (!this.isValid) {
            this.isValid = true;
            if (this.noodleElement != null) {
                this.noodleElement.classList.remove("noodle-invalid");
            }
        }
    }

    setInvalid = () => {
        this.isValid = false;
        if (this.noodleElement != null) {
            this.noodleElement.classList.add("noodle-invalid");
        }
    }

    static getNoodlePath = (startX: number, startY: number, endX: number, endY: number) => {
        var ctrlLength = (5 + 0.4 * Math.abs(endX - startX) + Math.min(0.2 * Math.abs(endY - startY), 40));
        var result = `M ${startX} ${startY} C ${startX + ctrlLength} ${startY} ${endX - ctrlLength} ${endY} ${endX} ${endY}`;
        return result;
    }

    clearDragImage = (event:DragEvent) => {
        let img = new Image();
        img.src = 'data:image/gif;base64,R0lGODlhAQABAIAAAAUEBAAAACwAAAAAAQABAAACAkQBADs='; //A transparent image
        event.dataTransfer.setDragImage(img, 0, 0);
    }
}

window['tempNoodle'] = new NoodleDragHandler();