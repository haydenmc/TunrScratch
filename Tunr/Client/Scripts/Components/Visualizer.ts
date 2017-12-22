import { Component } from "../Component";

export class Visualizer extends Component {
    constructor() {
        super("Visualizer");
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
        context.fillStyle = "#ff0000";
        context.fillRect(0, 0, context.canvas.width, context.canvas.height);
        requestAnimationFrame(() => this.draw());
    }
}
