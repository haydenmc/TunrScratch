import { Tunr } from "../../Tunr";
import { Component } from "../../Component";
import { EventHandler } from "../../EventHandler";
import { IObservable, ValueChangedEvent } from "../../Data/IObservable";
import { ObservableArray, ObservableArrayEventArgs } from "../../Data/ObservableArray";

export abstract class FilterLibraryView extends Component {
    public readonly onSelected: EventHandler<string> = new EventHandler<string>();
    private propertyValues: IObservable<ObservableArray<string>>;

    // Event handler methods
    private readonly onPropertyValuesChanged = (arg: ValueChangedEvent<ObservableArray<string>>) => {
        this.unsubscribeEventHandlers(arg.oldValue);
        this.subscribeEventHandlers(arg.newValue);
        this.renderData(arg.newValue);
    };
    private readonly onPropertyValuesItemAdded = (arg: ObservableArrayEventArgs<string>) => {
        this.itemAdded(arg);
    }
    private readonly onPropertyValuesItemRemoved = (arg: ObservableArrayEventArgs<string>) => {
        this.itemRemoved(arg);
    }

    constructor(componentName: string, propertyValues: IObservable<ObservableArray<string>>) {
        super(componentName);
        this.propertyValues = propertyValues;
        this.subscribeEventHandlers(this.propertyValues.value);
        this.propertyValues.onValueChanged.subscribe(this.onPropertyValuesChanged);
    }

    private subscribeEventHandlers(newValue: ObservableArray<string>): void {
        newValue.itemAdded.subscribe(this.onPropertyValuesItemAdded);
        newValue.itemRemoved.subscribe(this.onPropertyValuesItemRemoved);
    }

    private unsubscribeEventHandlers(oldValue: ObservableArray<string>): void {
        oldValue.itemAdded.unSubscribe(this.onPropertyValuesItemAdded);
        oldValue.itemRemoved.unSubscribe(this.onPropertyValuesItemRemoved);
    }

    protected abstract renderData(arg: ObservableArray<string>): void;

    protected abstract itemAdded(arg: ObservableArrayEventArgs<string>): void;

    protected abstract itemRemoved(arg: ObservableArrayEventArgs<string>): void;
}
