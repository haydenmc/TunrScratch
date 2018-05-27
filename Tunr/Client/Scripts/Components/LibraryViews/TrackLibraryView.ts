import { Component } from "../../Component";
import { EventHandler } from "../../EventHandler";
import { IObservable, ValueChangedEvent } from "../../Data/IObservable";
import { ObservableArray, ObservableArrayEventArgs } from "../../Data/ObservableArray";
import { ITrackModel } from "../../Data/Models/ITrackModel";

export class TrackLibraryView extends Component {
    public readonly onSelected: EventHandler<ITrackModel> = new EventHandler<ITrackModel>();
    private tracks: IObservable<ObservableArray<ITrackModel>>;

    // Event handler methods
    private readonly onPropertyValuesChanged = (arg: ValueChangedEvent<ObservableArray<ITrackModel>>) => {
        this.unsubscribeEventHandlers(arg.oldValue);
        this.subscribeEventHandlers(arg.newValue);
        this.renderData(arg.newValue);
    };
    private readonly onPropertyValuesItemAdded = (arg: ObservableArrayEventArgs<ITrackModel>) => {
        this.itemAdded(arg);
    }
    private readonly onPropertyValuesItemRemoved = (arg: ObservableArrayEventArgs<ITrackModel>) => {
        this.itemRemoved(arg);
    }

    constructor(tracks: IObservable<ObservableArray<ITrackModel>>) {
        super("TrackLibraryView");
        this.tracks = tracks;
        this.subscribeEventHandlers(this.tracks.value);
        this.tracks.onValueChanged.subscribe(this.onPropertyValuesChanged);
    }

    private subscribeEventHandlers(newValue: ObservableArray<ITrackModel>): void {
        newValue.itemAdded.subscribe(this.onPropertyValuesItemAdded);
        newValue.itemRemoved.subscribe(this.onPropertyValuesItemRemoved);
    }

    private unsubscribeEventHandlers(oldValue: ObservableArray<ITrackModel>): void {
        oldValue.itemAdded.unSubscribe(this.onPropertyValuesItemAdded);
        oldValue.itemRemoved.unSubscribe(this.onPropertyValuesItemRemoved);
    }

    protected renderData(arg: ObservableArray<ITrackModel>): void {
        let listElement = this.element.querySelector("ul");
        listElement.innerHTML = "";
        for(let i = 0; i < arg.size; i++) {
            let propertyValue = arg.get(i);
            let propertyElement = document.createElement("li");
            propertyElement.innerText = propertyValue.tagTitle;
            propertyElement.addEventListener("click", () => {
                this.onSelected.fire(propertyValue);
            });
            listElement.appendChild(propertyElement);
        }
    }

    protected itemAdded(arg: ObservableArrayEventArgs<ITrackModel>): void {
        throw new Error("Method not implemented.");
    }

    protected itemRemoved(arg: ObservableArrayEventArgs<ITrackModel>): void {
        throw new Error("Method not implemented.");
    }
}
