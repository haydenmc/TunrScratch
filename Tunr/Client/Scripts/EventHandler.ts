/**
 * A simple event handler.
 * Maintains a set of callbacks to fire upon some event.
 */
export class EventHandler<T> {
    private callbacks: Array<(arg: T) => void>;

    constructor() {
        this.callbacks = new Array<(arg: T) => void>();
    }

    public subscribe(callback: (arg: T) => void) {
        this.callbacks.push(callback);
    }

    public unSubscribe(callback: (arg: T) => void) {
        var index = this.callbacks.indexOf(callback);
        if (index >= 0) {
            this.callbacks.splice(index, 1);
        }
    }

    public unSubscribeAll() {
        this.callbacks = new Array<(arg: T) => void>();
    }

    public fire(arg: T) {
        for (var i = 0; i < this.callbacks.length; i++) {
            this.callbacks[i](arg);
        }
    }
}
