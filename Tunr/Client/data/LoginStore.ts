import { observable } from "mobx";

export class LoginStore {
    /**
     * Email the user has entered for authentication.
     */
    @observable
    public email: string = "";

    /**
     * Password the user has entered for authentication.
     */
    @observable
    public password: string = "";

    /**
     * Error text returned from the server in the event a request is rejected.
     */
    @observable
    public errorText: string = "";

    /**
     * Whether or not we have successfully requested an authentication token from the server.
     */
    @observable
    public isAuthenticated: boolean = false;

    /**
     * Authentication token used to authorize requests to the server.
     */
    @observable
    public token: string = "";

    /**
     * The date the authentication token becomes expired.
     */
    @observable
    public tokenExpires: Date = undefined;

    /**
     * The GUID of the user we're authenticated as.
     */
    @observable
    public userId: string = "";

    /**
     * Whether or not the email last processed belongs to an existing account.
     */
    @observable
    public doesLoginEmailBelongToAccount: boolean = undefined;

    /**
     * Whether or not LoginStore is busy processing a request.
     */
    @observable
    public isBusy: boolean = false;

    /**
     * Submits credentials to server to retrieve authentication token.
     */
    public processLogin(): void {
        this.isBusy = true;
        fetch("/Token", { 
                body: JSON.stringify({ email: this.email, password: this.password }),
                method: "POST",
                headers: { "Content-Type": "application/json" }
            }).then((response) => {
                if (response.ok) {
                    return response.json().then((tokenInfo) => {
                        this.isAuthenticated = tokenInfo.authenticated;
                        this.userId = tokenInfo.entityId;
                        this.token = tokenInfo.token;
                        this.tokenExpires = new Date(tokenInfo.tokenExpires);
                        this.isBusy = false;
                    });
                } else {
                    return response.text().then((text) => {
                        throw Error("Couldn't log in. Please check your email address or password.");
                    });
                }
            }).catch((error: Error) => {
                this.errorText = error.message;
                this.isBusy = false;
            });
    }
}