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

    constructor(tunrInstance: Tunr) {
        super("PlayerPage", tunrInstance);
    }

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        Animator.applyAnimationClass(this.element, "animation-zoom-forward-in");
        this.visualizer = new Visualizer(this.tunrInstance);
        this.visualizer.insertComponent(this.element.querySelector(".visualizerContainer"));
        this.libraryBlur = new BlurTarget(this.tunrInstance, this.visualizer.canvas, 32, 64, "#ffffff", 0.6);
        this.libraryBlur.insertComponent(this.element.querySelector("section.library"));
        this.playlistBlur = new BlurTarget(this.tunrInstance, this.visualizer.canvas, 8, 48, "#ffffff", 0.3);
        this.playlistBlur.insertComponent(this.element.querySelector("section.playlist"), this.element.querySelector("section.playlist").firstChild);
        this.libraryPane = new LibraryPane(this.tunrInstance);
        this.libraryPane.insertComponent(this.element.querySelector("section.library"));
        this.playlistPane = new PlaylistPane(this.tunrInstance);
        this.playlistPane.insertComponent(this.element.querySelector("section.playlist"));
        this.playingPane = new PlayingPane(this.tunrInstance);
        this.playingPane.insertComponent(this.element.querySelector("section.playing"));
    }
}
