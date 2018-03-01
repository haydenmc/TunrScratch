import { IObservable, ValueChangedEvent } from "./IObservable";
import { EventHandler } from "../EventHandler";

/**
 * A simple value store that notifies any subscribers of changes to its value.
 */
export class Observable<T> implements IObservable<T> {
    private _value: T;

    public get value(): T {
        return this._value;
    }

    public set value(newVal: T) {
        if (this._value !== newVal) {
            var oldVal = this._value;
            this._value = newVal;
            this._onValueChanged.fire({ oldValue: oldVal, newValue: newVal });
        }
    }

    private _onValueChanged: EventHandler<ValueChangedEvent<T>>;

    public get onValueChanged(): EventHandler<ValueChangedEvent<T>> {
        return this._onValueChanged;
    }

    constructor(defaultValue?: T) {
        this._onValueChanged = new EventHandler<ValueChangedEvent<T>>();
        this._value = defaultValue;
    }
}
