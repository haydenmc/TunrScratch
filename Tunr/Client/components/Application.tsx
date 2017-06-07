import * as React from "react";
import { ApplicationStore } from "../data/ApplicationStore";
import { Login } from "./Login";

export class Application extends React.Component<undefined, undefined> {
    private applicationStore: ApplicationStore
        = new ApplicationStore();

    constructor() {
        super();
    }

    render() {
        return (
            <Login applicationStore={this.applicationStore} />
        );
    }
}