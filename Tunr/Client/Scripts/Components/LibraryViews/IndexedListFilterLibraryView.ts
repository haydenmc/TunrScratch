import { FilterLibraryView } from "./FilterLibraryView";
import { IObservable } from "../../Data/IObservable";
import { ObservableArray, ObservableArrayEventArgs } from "../../Data/ObservableArray";

export class IndexedListFilterLibraryView extends FilterLibraryView {
    constructor(propertyValues: IObservable<ObservableArray<string>>) {
        super("IndexedListFilterLibraryView", propertyValues);
    }

    protected renderData(arg: ObservableArray<string>): void {
        this.element.innerHTML = "";
        for(var i = 0; i < arg.size; i++) {
            var propertyValue = arg.get(i);
            var propertyElement = document.createElement("li");
            propertyElement.innerText = propertyValue;
            this.element.appendChild(propertyElement);
        }
    }

    protected itemAdded(arg: ObservableArrayEventArgs<string>): void {
        //throw new Error("Method not implemented.");
    }

    protected itemRemoved(arg: ObservableArrayEventArgs<string>): void {
        //throw new Error("Method not implemented.");
    }
}
