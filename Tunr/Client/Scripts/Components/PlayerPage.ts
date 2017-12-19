import { Component } from "../Component";

export class PlayerPage extends Component {
    private authToken: TokenResponse; // User authentication token

    constructor(authToken: TokenResponse) {
        super("PlayerPage");
        this.authToken = authToken;
    }
}