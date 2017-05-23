import * as React from "react";
import { Login } from "./Login";

interface ApplicationProps {
    /* This space intentionally left blank */
}

interface ApplicationState {
    /* This space intentionally left blank */
}

export class Application extends React.Component<ApplicationProps, ApplicationState> {
    render() {
        return (
            <Login />
        );
    }
}