import { SignInPage } from "./Components/SignInPage";
import { PlayerPage } from "./Components/PlayerPage";

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
        if (window.location.pathname.length > 1) {
            if (window.location.pathname.toLowerCase() === "/player") {
                let player = new PlayerPage(<TokenResponse>{});
                player.insertComponent(document.body);
            }
        } else {
            // Show sign in
            let signIn = new SignInPage();
            signIn.insertComponent(document.body);
        }
    }
}

window.addEventListener("load", () => {
    Tunr.instance.start();
});
