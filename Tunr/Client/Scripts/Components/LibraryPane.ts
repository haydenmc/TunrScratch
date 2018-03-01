import { Component } from "../Component";
import { Tunr } from "../Tunr";

export class LibraryPane extends Component {
    constructor(tunrInstance: Tunr) {
        super("LibraryPane", tunrInstance);
    }
}
