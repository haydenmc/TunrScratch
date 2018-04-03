import { Component } from "../Component";
import { Tunr } from "../Tunr";
import { IndexedListFilterLibraryView } from "./LibraryViews/IndexedListFilterLibraryView";
import { FilterLibraryView } from "./LibraryViews/FilterLibraryView";

export class LibraryPane extends Component {
    private filterViews: FilterLibraryView[] = new Array<FilterLibraryView>();
    private filterValues: string[] = [];
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
        // Initialize new list component
        var newList = this.createComponent<IndexedListFilterLibraryView>(
            IndexedListFilterLibraryView,
            this.dataModel.fetchFilterPropertyValues(this.currentLibraryTreeViewIndex, this.filters)
        )
        newList.onSelected.subscribe(
            (selectedValue) => this.filterSelected(this.currentLibraryTreeViewIndex, selectedValue));
        // Pull out last list element
        this.filterViews[this.filterViews.length - 1].removeComponent();
        // Insert new list element
        newList.insertComponent(this.element);
        this.filterViews.push(newList);
    }
}
