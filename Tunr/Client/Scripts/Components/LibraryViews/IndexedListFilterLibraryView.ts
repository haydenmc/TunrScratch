import { FilterLibraryView } from "./FilterLibraryView";
import { IObservable } from "../../Data/IObservable";
import { ObservableArray, ObservableArrayEventArgs } from "../../Data/ObservableArray";

export class IndexedListFilterLibraryView extends FilterLibraryView {
    constructor(propertyValues: IObservable<ObservableArray<string>>) {
        super("IndexedListFilterLibraryView", propertyValues);
    }

    protected renderData(arg: ObservableArray<string>): void {
        let listElement = this.element.querySelector("ul");
        listElement.innerHTML = "";
        for(let i = 0; i < arg.size; i++) {
            let propertyValue = arg.get(i);
            let propertyElement = document.createElement("li");
            propertyElement.innerText = propertyValue;
            propertyElement.addEventListener("click", () => {
                this.onSelected.fire(propertyValue);
            });
            listElement.appendChild(propertyElement);
        }
    }

    protected itemAdded(arg: ObservableArrayEventArgs<string>): void {
        throw new Error("Method not implemented.");
    }

    protected itemRemoved(arg: ObservableArrayEventArgs<string>): void {
        throw new Error("Method not implemented.");
    }
}
