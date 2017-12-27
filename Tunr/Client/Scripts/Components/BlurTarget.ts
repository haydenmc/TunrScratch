import { Component } from "../Component";

export class BlurTarget extends Component {
    private source: HTMLCanvasElement;
    private blur: number = 32;

    constructor(source: HTMLCanvasElement, blur?: number) {
        super("BlurTarget");
        this.source = source;
        if (blur) {
            this.blur = blur;
        }
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);

        // Adjust canvas bounds
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        canvas.width = canvas.parentElement.clientWidth;
        canvas.height = canvas.parentElement.clientHeight;

        // Adjust bounds on window resize
        window.addEventListener("resize", () => {
            let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
            canvas.width = canvas.parentElement.clientWidth;
            canvas.height = canvas.parentElement.clientHeight;
        });

        // Set blur
        this.element.style.filter = "blur(" + this.blur + "px)";

        // Begin drawing
        this.draw();
    }

    private draw(): void {
        // Draw
        let canvas: HTMLCanvasElement = <HTMLCanvasElement>this.element;
        let context = canvas.getContext("2d");
        context.save();
        context.globalCompositeOperation = "copy";
        context.drawImage(this.source, 0, 0);
        context.restore();
        requestAnimationFrame(() => this.draw());
    }
}
