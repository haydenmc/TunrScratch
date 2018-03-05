import { Tunr } from "../../Tunr";
import { Component } from "../../Component";
import { EventHandler } from "../../EventHandler";

export abstract class FilterLibraryView extends Component {
    private propertyValues: string[];
    public readonly onSelected: EventHandler<string> = new EventHandler<string>();

    constructor(componentName: string) {
        super(componentName);
    }

    public setData(propertyValues: string[]) {
        this.propertyValues = propertyValues;
    }
}
