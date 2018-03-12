import { SignInPage } from "./Components/SignInPage";
import { PlayerPage } from "./Components/PlayerPage";
import { Component } from "./Component";
import { DataModel } from "./Data/DataModel";
import { RestRequest, RestRequestMethod } from "./Data/RestRequest";
import { ILoginResponse } from "./Data/Models/ILoginResponse";

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
        // This is where all the magic begins
        console.log("🔊 TUNR");

        // Find the splash screen (we'll need to remove it later)
        let splashElement = document.body.querySelector("#Splash");

        // Are we signed in?
        RestRequest.jsonRequest<ILoginResponse>(DataModel.UserInfoEndpoint, RestRequestMethod.Get).then(
            (response) => {
                // Request succeeded - we're already signed in
                this.dataModel.loginResponse.value = response;
                splashElement.parentNode.removeChild(splashElement);
                // Go to the player
                let player = Component.createComponent<PlayerPage>(PlayerPage, this);
                player.insertComponent(document.body);
            },
            (reason) => {
                // Request failed for one reason or another - we're probably not signed in
                splashElement.parentNode.removeChild(splashElement);
                // Show sign in
                let signIn = Component.createComponent<SignInPage>(SignInPage, this);
                signIn.insertComponent(document.body);
            }
        );
    }
}

window.addEventListener("load", () => {
    Tunr.instance.start();
});
