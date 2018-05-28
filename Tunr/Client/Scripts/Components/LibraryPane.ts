import { Component } from "../Component";
import { Tunr } from "../Tunr";
import { IndexedListFilterLibraryView } from "./LibraryViews/IndexedListFilterLibraryView";
import { FilterLibraryView } from "./LibraryViews/FilterLibraryView";
import { TrackLibraryView } from "./LibraryViews/TrackLibraryView";
import { ITrackModel } from "../Data/Models/ITrackModel";

/**
 * The LibraryPane is where the user browses their library tree-style (artist->album->track).
 * This class is responsible for managing traversal of this library tree
 * and presenting the appropriate views for each level.
 * 
 * There are two types of library views - filter views (artist->album) and track views (->track).
 * The last node/leaf of the library tree is always a track view
 * (since the user ultimately must select a track).
 * The non-leaf nodes are always filter views.
 */
export class LibraryPane extends Component {

    /** The current level of the tree being displayed */
    private currentLibraryTreeViewIndex: number = 0;

    /** An ordered list (per tree index) of filter views that have been displayed/are being displayed */
    private filterViews: FilterLibraryView[] = new Array<FilterLibraryView>();

    /** An ordered list (per tree index) of filter values that have been selected by the user */
    private filterValues: string[] = [];

    /** The track view being displayed after users have traversed all filter levels */
    private trackView: TrackLibraryView;

    /** Returns a map of filter names to filter values selected by the user */
    private get filters() {
        var retVal: any = { };
        for (var i = 0; i < this.filterValues.length; i++) {
            retVal[this.dataModel.filterPropertyNames[i]] = this.filterValues[i];
        }
        return retVal;
    }

    /**
     * Registers the component
     * @constructor
     */
    constructor() {
        super("LibraryPane");
    }

    /**
     * Initializes the component and populates the first filter view
     */
    public initialize(): void {
        super.initialize();

        // Initialize first library view
        var firstList = this.createComponent<IndexedListFilterLibraryView>(
            IndexedListFilterLibraryView,
            this.dataModel.fetchFilterPropertyValues(this.currentLibraryTreeViewIndex));
        firstList.onSelected.subscribe(
            (selectedValue) => this.filterSelected(this.currentLibraryTreeViewIndex, selectedValue));
        firstList.insertComponent(this.element.querySelector(".views"));
        this.filterViews.push(firstList);

        // Bind the header to navigate to the library root
        this.element.querySelector("h1").addEventListener("click", () => {
            this.navigateToIndex(0);
        });
    }

    /**
     * Register a selection for a filter view at a particular tree level and advance the tree to the next level
     * @param index The index of the tree at which this filter value was selected
     * @param value The value selected for this filter
     */
    private filterSelected(index: number, value: string): void {

        // Update internal state
        var newIndex = index + 1;
        this.currentLibraryTreeViewIndex = newIndex;
        this.filterValues.push(value);

        // Determine where we are in the tree
        if (this.currentLibraryTreeViewIndex < this.dataModel.filterPropertyNames.length) {

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
            newList.insertComponent(this.element.querySelector(".views"));
            this.filterViews.push(newList);

        } else if (this.currentLibraryTreeViewIndex == this.dataModel.filterPropertyNames.length) {

            // We've exhausted filters - time to display tracks
            var trackList = this.createComponent<TrackLibraryView>(
                TrackLibraryView,
                this.dataModel.fetchTracks(this.filters)
            );
            trackList.onSelected.subscribe(
                (selectedValue) => this.trackSelected(selectedValue));

            // Pull out last list element
            this.filterViews[this.filterViews.length - 1].removeComponent();

            // Insert new list element
            trackList.insertComponent(this.element.querySelector(".views"));
            this.trackView = trackList;

        } else {

            // Uh oh...
            // TODO handle unexpected library tree state
        }

        // Update back stack
        var newBackStackItem = document.createElement("li");
        newBackStackItem.innerText = value;
        newBackStackItem.addEventListener("click", () => {
            this.navigateToIndex(newIndex);
        });
        var lastBackStackElement = this.element.querySelector(".backStack").lastElementChild;
        if (lastBackStackElement) {
            lastBackStackElement.classList.add("inactive");
        }
        this.element.querySelector(".backStack").appendChild(newBackStackItem);

        // Mark header as inactive
        this.element.querySelector("h1").classList.add("inactive");
    }

    /**
     * Used to navigate back up the library tree to a particular index.
     * @param index Index of the tree to navigate to
     */
    private navigateToIndex(index: number): void {

        // Make sure we're actually going somewhere we can go
        if (index < this.currentLibraryTreeViewIndex) {

            // Remove the currently showing view
            if (this.currentLibraryTreeViewIndex < this.dataModel.filterPropertyNames.length) {

                // Filter view
                this.filterViews[this.filterViews.length - 1].removeComponent();

            } else if (this.currentLibraryTreeViewIndex == this.dataModel.filterPropertyNames.length) {

                // Track view
                this.trackView.removeComponent();

            } else {

                // Uh oh...
                // TODO handle unexpected library tree state
            }
            
            // Reset track view (track view will never be our destination when navigating)
            this.trackView = null;

            // Reset filter values (all after specified index)
            this.filterValues.splice(index);

            // Reset filter views (all after specified index) (values and views are off by 1)
            this.filterViews.splice(index + 1);

            // Update index state
            this.currentLibraryTreeViewIndex = index;

            // Reset back stack
            var backStackElements = this.element.querySelector(".backStack").children;
            for (var i = backStackElements.length - 1; i >= index; i--) {
                backStackElements.item(i).remove();
            }
            var lastBackStackElement = this.element.querySelector(".backStack").lastElementChild;
            if (lastBackStackElement) {
                lastBackStackElement.classList.remove("inactive");
            }

            // Mark header as active if we're back at root
            if (index === 0) {
                this.element.querySelector("h1").classList.remove("inactive");
            }

            // Display appropriate filter view
            this.filterViews[this.filterViews.length - 1].insertComponent(this.element.querySelector(".views"));
        }
    }

    /**
     * Register a selection for the track view.
     * @param value The track that was selected
     */
    private trackSelected(value: ITrackModel): void
    {
        // TODO
    }
}
