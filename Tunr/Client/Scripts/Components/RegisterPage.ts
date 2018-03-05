import { Component } from "../Component";
import { SignInPage } from "./SignInPage";
import { Tunr } from "../Tunr";

export class RegisterPage extends Component {
    constructor() {
        super("RegisterPage");
    }

    public initialize(): void {
        super.initialize();
        this.element.querySelector("form").addEventListener("submit", (e: Event) => {
            e.preventDefault();
            this.registerPressed();
        });
        this.element.querySelector("button[name='signin']").addEventListener("click", (e: Event) => {
            e.preventDefault();
            this.signInPressed();
        });
        this.element.querySelector("button[name='register']").addEventListener("click", (e: Event) => {
            e.preventDefault();
            this.registerPressed();
        });
    }

    protected componentInserted(): void {
        // Focus the email field when this component is inserted
        let emailInput: HTMLInputElement = this.element.querySelector("input[name=email]");
        emailInput.focus();
    }

    private signInPressed(): void {
        // TODO: Consider a navigation service to preserve old page instances
        let signInPage = this.createComponent<SignInPage>(SignInPage);
        let parentElement = this.element.parentElement;
        this.removeComponent();
        signInPage.insertComponent(parentElement);
    }

    private registerPressed(): void {
        // Get input elements
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
        // Make sure we're ready to submit
        if (emailField.disabled || passwordField.disabled) {
            return;
        }
        // Get input values
        let email = emailField.value;
        let password = passwordField.value;
        // TODO: Validation
        // Set form state
        emailField.disabled = true;
        passwordField.disabled = true;
        // Submit registration
        let requestBody = JSON.stringify({
            email: email,
            password: password
        });
        let requestHeaders = new Headers();
        requestHeaders.append("Content-Type", "application/json");
        requestHeaders.append("Content-Length", requestBody.length.toString());
        fetch("/User", {
            method: "POST",
            headers: requestHeaders,
            body: requestBody
        }).then(
            (response) => this.onRegisterRequestFulfilled(response),
            (reason) => this.onRegisterRequestRejected(reason)
        );
    }

    private onRegisterRequestFulfilled(response: Response): void {
        // Get input elements
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
        // Process response
        if (response.ok) {
            alert("Registration successful!");
            // Navigate to login
            let signInPage = this.createComponent<SignInPage>(SignInPage);
            let parentElement = this.element.parentElement;
            this.removeComponent();
            signInPage.insertComponent(parentElement);
        } else {
            alert("Error processing registration: " + response.status + "/" + response.statusText);
            emailField.disabled = false;
            passwordField.disabled = false;
        }
    }

    private onRegisterRequestRejected(reason: any): void {
        alert("Error processing registration.");
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
        emailField.disabled = false;
        passwordField.disabled = false;
    }
}
