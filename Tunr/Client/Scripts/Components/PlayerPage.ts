import { Component } from "../Component";
import { Visualizer } from "./Visualizer";
import { BlurTarget } from "./BlurTarget";

export class PlayerPage extends Component {
    private authToken: TokenResponse; // User authentication token
    private visualizer: Visualizer;
    private playerBlur: BlurTarget;

    constructor(authToken: TokenResponse) {
        super("PlayerPage");
        this.authToken = authToken;
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        this.visualizer = new Visualizer();
        this.visualizer.insertComponent(this.element, this.element.firstChild);
        this.playerBlur = new BlurTarget(this.visualizer.canvas);
        this.playerBlur.insertComponent(this.element.querySelector("section.library"), this.element.querySelector("section.library").firstChild);
    }
}
