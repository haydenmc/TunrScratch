import { EventHandler } from "./EventHandler";

export interface IObservable<T> {
    value: T;
    onValueChanged: EventHandler<ValueChangedEvent<T>>;
}

export interface ValueChangedEvent<T> {
    oldValue: T;
    newValue: T;
}
