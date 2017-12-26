import { Component } from "../Component";
import { Visualizer } from "./Visualizer";

export class PlayerPage extends Component {
    private authToken: TokenResponse; // User authentication token
    private visualizer: Visualizer;

    constructor(authToken: TokenResponse) {
        super("PlayerPage");
        this.authToken = authToken;
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        this.visualizer = new Visualizer();
        this.visualizer.insertComponent(this.element, this.element.firstChild);
    }
}
