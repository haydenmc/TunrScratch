import { Component } from "../Component";
import { Tunr } from "../Tunr";
import { IndexedListFilterLibraryView } from "./LibraryViews/IndexedListFilterLibraryView";
import { FilterLibraryView } from "./LibraryViews/FilterLibraryView";
import { TrackLibraryView } from "./LibraryViews/TrackLibraryView";

export class LibraryPane extends Component {
    private filterViews: FilterLibraryView[] = new Array<FilterLibraryView>();
    private filterValues: string[] = [];
    private trackView: TrackLibraryView;
    private currentLibraryTreeViewIndex: number = 0;
    private get filters() {
        var retVal: any = { };
        for (var i = 0; i < this.filterValues.length; i++) {
            retVal[this.dataModel.filterPropertyNames[i]] = this.filterValues[i];
        }
        return retVal;
    }

    constructor() {
        super("LibraryPane");
    }

    public initialize(): void {
        super.initialize();

        // Initialize first library view
        var firstList = this.createComponent<IndexedListFilterLibraryView>(
            IndexedListFilterLibraryView,
            this.dataModel.fetchFilterPropertyValues(this.currentLibraryTreeViewIndex));
        firstList.onSelected.subscribe(
            (selectedValue) => this.filterSelected(this.currentLibraryTreeViewIndex, selectedValue));
        firstList.insertComponent(this.element);
        this.filterViews.push(firstList);
    }

    private filterSelected(index: number, value: string): void {
        // Update internal state
        this.currentLibraryTreeViewIndex = index + 1;
        this.filterValues.push(value);
        // Determine where we are in the tree
        if (this.currentLibraryTreeViewIndex < this.dataModel.filterPropertyNames.length)
        {
            // There are still more filters left in the tree
            // Initialize new list component
            var newList = this.createComponent<IndexedListFilterLibraryView>(
                IndexedListFilterLibraryView,
                this.dataModel.fetchFilterPropertyValues(this.currentLibraryTreeViewIndex, this.filters)
            );
            newList.onSelected.subscribe(
                (selectedValue) => this.filterSelected(this.currentLibraryTreeViewIndex, selectedValue));
            // Pull out last list element
            this.filterViews[this.filterViews.length - 1].removeComponent();
            // Insert new list element
            newList.insertComponent(this.element);
            this.filterViews.push(newList);
        }
        else if (this.currentLibraryTreeViewIndex == this.dataModel.filterPropertyNames.length)
        {
            // We've exhausted filters - time to display tracks
            var trackList = this.createComponent<TrackLibraryView>(
                TrackLibraryView,
                this.dataModel.fetchTracks(this.filters)
            );
        }
        else {
            // Uh oh...
        }
    }
}
