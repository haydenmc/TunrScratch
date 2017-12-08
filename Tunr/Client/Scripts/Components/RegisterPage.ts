import { Component } from "../Component";
import { SignInPage } from "./SignInPage";

export class RegisterPage extends Component {
    constructor() {
        super("RegisterPage");
        this.element.querySelector("button[name=signin]").addEventListener("click", (e) => {
            e.preventDefault();
            this.signInPressed();
        });
    }

    protected componentInserted(): void {
        // Focus the email field when this component is inserted
        let emailInput: HTMLInputElement = this.element.querySelector("input[name=email]");
        emailInput.focus();
    }

    private signInPressed(): void {
        // TODO: Consider a navigation service to preserve old page instances
        let signInPage = new SignInPage();
        let parentElement = this.element.parentElement;
        this.removeComponent();
        signInPage.insertComponent(parentElement);
    }
}
