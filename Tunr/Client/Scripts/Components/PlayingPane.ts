import { Component } from "../Component";
import { Tunr } from "../Tunr";

export class PlayingPane extends Component {
    constructor(tunrInstance: Tunr) {
        super("PlayingPane", tunrInstance);
    }
}
