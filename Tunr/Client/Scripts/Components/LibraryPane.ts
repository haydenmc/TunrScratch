import { Component } from "../Component";
import { Tunr } from "../Tunr";
import { IndexedListFilterLibraryView } from "./LibraryViews/IndexedListFilterLibraryView";
import { FilterLibraryView } from "./LibraryViews/FilterLibraryView";

export class LibraryPane extends Component {
    private artistList: FilterLibraryView;

    constructor() {
        super("LibraryPane");
    }

    public initialize(): void {
        super.initialize();
        this.artistList = this.createComponent<IndexedListFilterLibraryView>(IndexedListFilterLibraryView, this.dataModel.artists);
        this.artistList.insertComponent(this.element);
        this.dataModel.fetchArtists();
    }
}
