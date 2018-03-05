import { Component } from "../Component";
import { Visualizer } from "./Visualizer";
import { BlurTarget } from "./BlurTarget";
import { Animator } from "../Animator";
import { LibraryPane } from "./LibraryPane";
import { PlaylistPane } from "./PlaylistPane";
import { PlayingPane } from "./PlayingPane";
import { Tunr } from "../Tunr";

export class PlayerPage extends Component {
    private visualizer: Visualizer;
    private libraryBlur: BlurTarget;
    private playlistBlur: BlurTarget;
    private libraryPane: LibraryPane;
    private playlistPane: PlaylistPane;
    private playingPane: PlayingPane;

    constructor() {
        super("PlayerPage");
    }

    public initialize(): void {
        super.initialize();
        console.log("Hey, you're " + this.dataModel.loginResponse.value.email + " right?");
        // Sub-components
        // Main visualizer (sits behind everything)
        this.visualizer = this.createComponent<Visualizer>(Visualizer);
        this.visualizer.insertComponent(this.element.querySelector(".visualizerContainer"));
        // Blur panes
        this.libraryBlur = this.createComponent<BlurTarget>(BlurTarget, this.visualizer.canvas, 32, 64, "#ffffff", 0.6);
        this.libraryBlur.insertComponent(this.element.querySelector("section.library"));
        this.playlistBlur = this.createComponent<BlurTarget>(BlurTarget, this.visualizer.canvas, 8, 48, "#ffffff", 0.3);
        this.playlistBlur.insertComponent(this.element.querySelector("section.playlist"), this.element.querySelector("section.playlist").firstChild);
        // Library/Playlist/Playing components
        this.libraryPane = this.createComponent<LibraryPane>(LibraryPane);
        this.libraryPane.insertComponent(this.element.querySelector("section.library"));
        this.playlistPane = this.createComponent<PlaylistPane>(PlaylistPane);
        this.playlistPane.insertComponent(this.element.querySelector("section.playlist"));
        this.playingPane = this.createComponent<PlayingPane>(PlayingPane);
        this.playingPane.insertComponent(this.element.querySelector("section.playing"));
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        Animator.applyAnimationClass(this.element, "animation-zoom-forward-in");
        this.visualizer.adjustCanvasBounds();
        this.libraryBlur.adjustCanvasBounds();
        this.playlistBlur.adjustCanvasBounds();
    }
}
