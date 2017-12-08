import { Component } from "../Component";
import { RegisterPage } from "./RegisterPage";

export class SignInPage extends Component {
    constructor() {
        super("SignInPage");
        this.element.querySelector("button[name=register]").addEventListener("click", (e: Event) => {
            e.preventDefault();
            this.registerPressed();
        });
    }

    protected componentInserted(): void {
        // Focus the email field when this component is inserted
        let emailInput: HTMLInputElement = this.element.querySelector("input[name=email]");
        emailInput.focus();
    }

    private registerPressed(): void {
        // TODO: Consider a navigation service to preserve old page instances
        let registerPage = new RegisterPage();
        let parentElement = this.element.parentElement;
        this.removeComponent();
        registerPage.insertComponent(parentElement);
    }
}
