import { Component } from "../Component";

export class Visualizer extends Component {
    private readonly imageScaleFactor: number = 1.25;
    private currentImage: HTMLImageElement;
    private currentImageIsLoaded: boolean;

    constructor() {
        super("Visualizer");
        // Test
        this.setImage("/Assets/Images/TestArtistImage.jpg");
    }

    public setImage(url: string): void {
        this.currentImageIsLoaded = false;
        this.currentImage = new Image();
        this.currentImage.src = url;
        this.currentImage.onload = () => {
            this.currentImageIsLoaded = true;
        };
    }

    protected componentInserted() {
        this.draw();
    }

    private draw(): void {
        // Adjust canvas bounds
        // TODO: This should be done on window resize event for perf
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        canvas.width = canvas.parentElement.clientWidth;
        canvas.height = canvas.parentElement.clientHeight;
        // Draw
        let context = canvas.getContext("2d");
        if (this.currentImageIsLoaded) {
            // calculate draw dimensions
            let canvasAspect = canvas.width / canvas.height;
            let imageAspect = this.currentImage.naturalWidth / this.currentImage.naturalHeight;
            let scaleFactor: number;
            if (imageAspect >= canvasAspect) {
                // scale to match canvas height
                scaleFactor = (canvas.height / this.currentImage.naturalHeight);
            } else {
                // scale to match canvas width
                scaleFactor = (canvas.width / this.currentImage.naturalWidth);
            }
            // Adjust scale factor for zoom
            scaleFactor *= this.imageScaleFactor;
            let destinationWidth = this.currentImage.naturalWidth * scaleFactor;
            let destinationHeight = this.currentImage.naturalHeight * scaleFactor;
            // Randomize padding
            //let startX = -1 * Math.random() * (destinationWidth - canvas.width);
            //let startY = -1 * Math.random() * (destinationHeight - canvas.height);
            context.drawImage(this.currentImage, 0, 0, destinationWidth, destinationHeight); // startX, startY
        }
        requestAnimationFrame(() => this.draw());
    }
}
