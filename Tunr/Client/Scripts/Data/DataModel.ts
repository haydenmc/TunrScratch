import { IObservable } from "./IObservable";
import { Observable } from "./Observable";
import { ILoginResponse } from "./Models/ILoginResponse";
import { ObservableArray } from "./ObservableArray";

export class DataModel {
    private static readonly TrackPropertiesEndpoint: string = "/Library/Track/Properties";

    private _loginResponse: IObservable<ILoginResponse> = new Observable<ILoginResponse>();
    public get loginResponse(): IObservable<ILoginResponse> {
        return this._loginResponse;
    }

    private _artists: IObservable<ObservableArray<string>> = new Observable<ObservableArray<string>>(new ObservableArray<string>());
    public get artists(): IObservable<ObservableArray<string>> {
        return this._artists;
    }

    public fetchArtists(): void {
        let requestBody = JSON.stringify({
            propertyName: "TagPerformers",
            filters: undefined
        });
        let requestHeaders = new Headers();
        requestHeaders.append("Content-Type", "application/json");
        requestHeaders.append("Content-Length", requestBody.length.toString());
        fetch(DataModel.TrackPropertiesEndpoint, {
            method: "POST",
            headers: requestHeaders,
            body: requestBody,
            credentials: "same-origin"
        }).then(
            (response) => {
                if (response.ok) {
                    response.json().then((value: string[]) => {
                        var newArtistList = new ObservableArray<string>();
                        for (var i = 0; i < value.length; i++)
                        {
                            newArtistList.push(value[i]);
                        }
                        this.artists.value = newArtistList;
                    })
                }
            },
            (reason) => {
                // Error
            }
        );
    }
}
