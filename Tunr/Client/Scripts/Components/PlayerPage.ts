import { Component } from "../Component";
import { Visualizer } from "./Visualizer";

export class PlayerPage extends Component {
    private authToken: TokenResponse; // User authentication token
    private visualizer: Visualizer;

    constructor(authToken: TokenResponse) {
        super("PlayerPage");
        this.authToken = authToken;
        this.visualizer = new Visualizer();
        this.visualizer.insertComponent(this.element, this.element.firstChild);
    }
}
