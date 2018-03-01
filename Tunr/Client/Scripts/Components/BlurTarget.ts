import { Component } from "../Component";
import { Tunr } from "../Tunr";

export class BlurTarget extends Component {
    private source: HTMLCanvasElement;
    private blur: number = 32;

    private edgeBufferPx: number = 64; // How much the canvas should bleed outside the edge of the parent element
    private overlayColor: string = "#ffffff";
    private overlayOpacity: number = 0.5;

    constructor(tunrInstance:Tunr, source: HTMLCanvasElement, blur?: number, edgeBufferPx?: number, overlayColor?: string, overlayOpacity?: number) {
        super("BlurTarget", tunrInstance);
        this.source = source;
        if (blur) {
            this.blur = blur;
        }
        if (edgeBufferPx) {
            this.edgeBufferPx = edgeBufferPx;
        }
        if (overlayColor) {
            this.overlayColor = overlayColor;
        }
        if (overlayOpacity) {
            this.overlayOpacity = overlayOpacity;
        }
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);

        // Adjust canvas bounds
        this.adjustCanvasBounds();

        // Adjust bounds on window resize
        window.addEventListener("resize", () => this.adjustCanvasBounds());

        // Set blur
        this.element.style.filter = "blur(" + this.blur + "px)";

        // Begin drawing
        this.draw();
    }

    private adjustCanvasBounds(): void {
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        canvas.width = canvas.parentElement.clientWidth + (this.edgeBufferPx * 2);
        canvas.height = canvas.parentElement.clientHeight + (this.edgeBufferPx * 2);

        // The canvas needs to bleed outside the bounds of its parent by edgeBufferPx pixels
        canvas.style.position = "absolute";
        canvas.style.top = (-1 * this.edgeBufferPx) + "px";
        canvas.style.right = "0";
        canvas.style.bottom = "0";
        canvas.style.left = (-1 * this.edgeBufferPx) + "px";
    }

    private draw(): void {
        // Figure out our offset
        var sourceRect = this.source.getBoundingClientRect();
        var targetRect = this.element.getBoundingClientRect();
        var deltaX = targetRect.left - sourceRect.left;
        var deltaY = targetRect.top - sourceRect.top;
        // Draw
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        let context = canvas.getContext("2d");
        context.save();
        context.globalCompositeOperation = "copy";
        context.drawImage(this.source, -1 * deltaX, -1 * deltaY);
        context.globalCompositeOperation = "hard-light";
        context.globalAlpha = this.overlayOpacity;
        context.fillStyle = this.overlayColor;
        context.fillRect(0, 0, canvas.width, canvas.height);
        context.restore();
        requestAnimationFrame(() => this.draw());
    }
}
