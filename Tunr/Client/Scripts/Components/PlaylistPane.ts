import { Component } from "../Component";
import { Tunr } from "../Tunr";

export class PlaylistPane extends Component {
    constructor(tunrInstance: Tunr) {
        super("PlaylistPane", tunrInstance);
    }
}
