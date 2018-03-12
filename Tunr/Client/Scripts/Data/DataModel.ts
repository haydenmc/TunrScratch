import { IObservable } from "./IObservable";
import { Observable } from "./Observable";
import { ILoginResponse } from "./Models/ILoginResponse";
import { ObservableArray } from "./ObservableArray";
import { RestRequest, RestRequestMethod } from "./RestRequest";

export class DataModel {
    public static readonly UserInfoEndpoint: string = "/User";
    public static readonly TrackPropertiesEndpoint: string = "/Library/Track/Properties";

    private _loginResponse: IObservable<ILoginResponse> = new Observable<ILoginResponse>();
    public get loginResponse(): IObservable<ILoginResponse> {
        return this._loginResponse;
    }

    private _artists: IObservable<ObservableArray<string>> = new Observable<ObservableArray<string>>(new ObservableArray<string>());
    public get artists(): IObservable<ObservableArray<string>> {
        return this._artists;
    }

    public fetchArtists(): void {
        RestRequest.jsonRequest<string[]>(DataModel.TrackPropertiesEndpoint, RestRequestMethod.Post ,{
            propertyName: "TagPerformers",
            filters: undefined
        }).then(
            // Success
            (value: string[]) => {
                this.artists.value = new ObservableArray<string>(value);
            },
            // Failure
            (reason) => {
                // TODO
            }
        );
    }
}
