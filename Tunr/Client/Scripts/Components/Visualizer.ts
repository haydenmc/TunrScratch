import { Component } from "../Component";
import { Tunr } from "../Tunr";

export class Visualizer extends Component {
    // Constants
    private readonly cImageScaleFactor: number = 1.1; // How far to scale the image beyond the bounds of the canvas
    private readonly cImageZoomFactorRange: number = 0.1; // How far (max) to zoom in/out
    private readonly cImageOpacity: number = 1; // Opacity
    private readonly cImageTransitionTime: number = 1000 * 15; // How long a transition should last in ms

    private edgeBufferPx: number = 64; // How much the canvas should bleed outside the edge of the parent element

    private currentImage: {
        imageElement: HTMLImageElement;
        isLoaded: boolean;
        startScale: number;
        endScale: number;
        startX: number;
        startY: number;
        endX: number;
        endY: number;
        startTime: number;
    };

    public get canvas(): HTMLCanvasElement {
        return <HTMLCanvasElement>this.element;
    }

    constructor(edgeBufferPx?: number) {
        super("Visualizer");
        if (edgeBufferPx) {
            this.edgeBufferPx = edgeBufferPx;
        }
    }

    public setImage(url: string): void {
        this.currentImage = {
            imageElement: new Image(),
            isLoaded: false,
            startScale: 0,
            endScale: 0,
            startX: 0,
            startY: 0,
            endX: 0,
            endY: 0,
            startTime: 0
        };
        this.currentImage.imageElement.onload = () => {
            let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
            this.calculateImageProps();
            this.currentImage.isLoaded = true;
        };
        this.currentImage.imageElement.src = url;
    }

    private calculateImageProps(): void {
        // calculate draw dimensions
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        let canvasAspect = canvas.width / canvas.height;
        let imageAspect
            = this.currentImage.imageElement.naturalWidth
            / this.currentImage.imageElement.naturalHeight;
        let fillScaleFactor: number;
        if (imageAspect >= canvasAspect) {
            // scale to match canvas height
            fillScaleFactor = (canvas.height / this.currentImage.imageElement.naturalHeight);
        } else {
            // scale to match canvas width
            fillScaleFactor = (canvas.width / this.currentImage.imageElement.naturalWidth);
        }
        // Determine start scaling multiplier
        this.currentImage.startScale = fillScaleFactor * this.cImageScaleFactor;
        // Determine end scaling multiplier (with zoom)
        let zoomLevel
            = this.cImageScaleFactor
            + (Math.random() * 2 * this.cImageZoomFactorRange)
            - this.cImageZoomFactorRange;
        this.currentImage.endScale = (fillScaleFactor * zoomLevel);
        // Determine start X position
        let startImageWidth
            = this.currentImage.startScale * this.currentImage.imageElement.naturalWidth;
        this.currentImage.startX
            = Math.random()
            * (canvas.width - startImageWidth);
        // Determine start Y position
        let startImageHeight
            = this.currentImage.startScale * this.currentImage.imageElement.naturalHeight;
        this.currentImage.startY
            = Math.random()
            * (canvas.height - startImageHeight);
        // Determine end X position
        let endImageWidth
            = this.currentImage.endScale * this.currentImage.imageElement.naturalWidth;
        this.currentImage.endX
            = Math.random()
            * (canvas.width - endImageWidth);
        // Determine end Y position
        let endImageHeight
            = this.currentImage.endScale * this.currentImage.imageElement.naturalHeight;
        this.currentImage.endY
            = Math.random()
            * (canvas.height - endImageHeight);
        // Reset transition time
        this.currentImage.startTime = performance.now();

        console.log(this.currentImage);
    }

    public initialize(): void {
        super.initialize();

        // Adjust bounds on window resize
        window.addEventListener("resize", () => this.adjustCanvasBounds());
    }

    public insertComponent(parentNode: Node, beforeNode?: Node) {
        super.insertComponent(parentNode, beforeNode);

        // Adjust canvas bounds
        this.adjustCanvasBounds();

        // Begin drawing
        this.draw();

        // Test image
        this.setImage("/Assets/Images/TestArtistImage2.jpg");
    }

    public adjustCanvasBounds(): void {
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
        // Draw
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        let context = canvas.getContext("2d");
        if (this.currentImage && this.currentImage.isLoaded) {
            let timeElapsed = performance.now() - this.currentImage.startTime;
            let timeMultiplier = timeElapsed / this.cImageTransitionTime;
            // Calculate drawing parameters
            let currentX: number, currentY: number, currentWidth: number, currentHeight: number;
            if (timeMultiplier < 1)
            {
                let deltaX
                    = this.currentImage.endX
                    - this.currentImage.startX;
                currentX
                    = this.currentImage.startX
                    + (deltaX * timeMultiplier);
                let deltaY
                    = this.currentImage.endY
                    - this.currentImage.startY;
                currentY
                    = this.currentImage.startY
                    + (deltaY * timeMultiplier);
                let deltaScaling
                    = this.currentImage.endScale
                    - this.currentImage.startScale;
                let currentScaling
                    = this.currentImage.startScale
                    + (deltaScaling * timeMultiplier);
                currentWidth
                    = this.currentImage.imageElement.naturalWidth
                    * currentScaling;
                currentHeight
                    = this.currentImage.imageElement.naturalHeight
                    * currentScaling;
            } else {
                currentX = this.currentImage.endX;
                currentY = this.currentImage.endY;
                currentWidth
                    = this.currentImage.endScale
                    * this.currentImage.imageElement.naturalWidth;
                currentHeight
                    = this.currentImage.endScale
                    * this.currentImage.imageElement.naturalHeight;
            }
            // Draw it
            context.save(); // save default state
            context.globalAlpha = this.cImageOpacity;
            context.globalCompositeOperation = "copy";
            context.drawImage(
                this.currentImage.imageElement,
                currentX,
                currentY,
                currentWidth,
                currentHeight
            );
            context.font = "900 196px Open Sans";
            context.fillStyle = "#ffffff";
            context.globalAlpha = 0.5;
            context.globalCompositeOperation = "overlay";
            context.fillText("TUNR ROCKS", 800, 900);
            context.restore();
        }
        requestAnimationFrame(() => this.draw());
    }
}
