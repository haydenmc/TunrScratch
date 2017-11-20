import { SignInPage } from "./Components/SignInPage";

/**
 * This is the entry point for the Tunr web client.
 */
export class Tunr {
    private static sInstance: Tunr = null;
    public static get instance(): Tunr {
        if (Tunr.sInstance == null)
        {
            Tunr.sInstance = new Tunr();
        }
        return Tunr.sInstance;
    }

    public start(): void {
        console.log("🔊 TUNR");
        // Kill the splash screen
        let splashElement = document.body.querySelector("#Splash");
        splashElement.parentNode.removeChild(splashElement);
        // Show sign in
        let signIn = new SignInPage();
        signIn.insertComponent(document.body);
    }
}

window.addEventListener("load", () => {
    Tunr.instance.start();
});
