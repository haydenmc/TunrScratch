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
     * Submits email to server to determine whether this is a new or existing account.
     */
    public processEmail(): void {
        this.isBusy = true; // Simulate busy time
        setTimeout(() => {
            this.doesLoginEmailBelongToAccount = true;
            this.isBusy = false;
        }, 1000);
    }
}