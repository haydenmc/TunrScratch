import { Component } from "../Component";
import { RegisterPage } from "./RegisterPage";
import { PlayerPage } from "./PlayerPage";
import { Animator } from "../Animator";
import { Tunr } from "../Tunr";
import { ILoginResponse } from "../Data/Models/ILoginResponse";

export class SignInPage extends Component {
    constructor() {
        super("SignInPage");
    }

    public initialize(): void {
        super.initialize();
        this.element.querySelector("form").addEventListener("submit", (e: Event) => {
            e.preventDefault();
            this.signInPressed();
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

    public insertComponent(parentNode: Node, beforeNode?: Node): void {
        super.insertComponent(parentNode, beforeNode);
        // Focus the email field when this component is inserted
        let emailInput: HTMLInputElement = this.element.querySelector("input[name=email]");
        emailInput.focus();
        // Animate in
        Animator.applyAnimationClass(this.element.querySelector("form"), "animation-zoom-forward-in");
    }

    private signInPressed(): void {
        // Get input fields
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
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
        fetch("/User/Login", {
            method: "POST",
            headers: requestHeaders,
            body: requestBody,
            credentials: "same-origin"
        }).then(
            (response) => this.onSignInRequestFulfilled(response),
            (reason) => this.onSignInRequestRejected(reason)
        );
    }

    private registerPressed(): void {
        // TODO: Consider a navigation service to preserve old page instances
        let registerPage = this.createComponent<RegisterPage>(RegisterPage);
        let parentElement = this.element.parentElement;
        this.removeComponent();
        registerPage.insertComponent(parentElement);
    }

    private onSignInRequestFulfilled(response: Response): void {
        // Get input elements
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
        // Process response
        if (response.ok) {
            response.json().then(
                (value: ILoginResponse) => {
                    // Update data model
                    this.dataModel.loginResponse.value = value;
                    // Navigate to player
                    let playerPage = this.createComponent<PlayerPage>(PlayerPage);
                    let parentElement = this.element.parentElement;
                    Animator.applyAnimationClass(this.element, "animation-zoom-forward-out").then(() => {
                        this.removeComponent();
                    });
                    playerPage.insertComponent(parentElement);
                },
                (reason) => {
                    alert("Error parsing login response.");
                    emailField.disabled = false;
                    passwordField.disabled = false;
                }
            );
        } else {
            alert("Error processing registration: " + response.status + "/" + response.statusText);
            emailField.disabled = false;
            passwordField.disabled = false;
        }
    }

    private onSignInRequestRejected(reason: any): void {
        // Get input elements
        let emailField: HTMLInputElement = this.element.querySelector("input[name='email']");
        let passwordField: HTMLInputElement = this.element.querySelector("input[name='password']");
        alert("Error sending sign-in request.");
        emailField.disabled = false;
        passwordField.disabled = false;
    }
}
