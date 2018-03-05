import { SignInPage } from "./Components/SignInPage";
import { PlayerPage } from "./Components/PlayerPage";
import { Component } from "./Component";
import { DataModel } from "./Models/DataModel";

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

    private _dataModel: DataModel = new DataModel();
    public get dataModel(): DataModel {
        return this._dataModel;
    }

    public start(): void {
        console.log("🔊 TUNR");
        // Kill the splash screen
        let splashElement = document.body.querySelector("#Splash");
        splashElement.parentNode.removeChild(splashElement);
        if (window.location.pathname.length > 1) {
            if (window.location.pathname.toLowerCase() === "/player") {
                let player = Component.createComponent<PlayerPage>(PlayerPage, this);
                player.insertComponent(document.body);
            }
        } else {
            // Show sign in
            let signIn = Component.createComponent<SignInPage>(SignInPage, this);
            signIn.insertComponent(document.body);
        }
    }
}

window.addEventListener("load", () => {
    Tunr.instance.start();
});
