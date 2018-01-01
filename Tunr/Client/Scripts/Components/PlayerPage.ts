import { Component } from "../Component";
import { Visualizer } from "./Visualizer";
import { BlurTarget } from "./BlurTarget";

export class PlayerPage extends Component {
    private authToken: TokenResponse; // User authentication token
    private visualizer: Visualizer;
    private libraryBlur: BlurTarget;
    private playlistBlur: BlurTarget;

    constructor(authToken: TokenResponse) {
        super("PlayerPage");
        this.authToken = authToken;
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        this.visualizer = new Visualizer();
        this.visualizer.insertComponent(this.element.querySelector(".visualizerContainer"));
        this.libraryBlur = new BlurTarget(this.visualizer.canvas, 32, 64, "#ffffff", 0.6);
        this.libraryBlur.insertComponent(this.element.querySelector("section.library"), this.element.querySelector("section.library").firstChild);
        this.playlistBlur = new BlurTarget(this.visualizer.canvas, 8, 48, "#ffffff", 0.3);
        this.playlistBlur.insertComponent(this.element.querySelector("section.playlist"), this.element.querySelector("section.playlist").firstChild);
    }
}
