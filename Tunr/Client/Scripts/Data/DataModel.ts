import { IObservable } from "./IObservable";
import { Observable } from "./Observable";
import { ILoginResponse } from "./Models/ILoginResponse";
import { ObservableArray } from "./ObservableArray";
import { RestRequest, RestRequestMethod } from "./RestRequest";
import { ITrackModel } from "./Models/ITrackModel";

export class DataModel {
    // Server endpoints
    public static readonly UserInfoEndpoint: string = "/User";
    public static readonly TrackPropertiesEndpoint: string = "/Library/Track/Properties";
    public static readonly TracksEndpoint: string = "/Library/Track";

    // Library tree props
    public filterPropertyNames: string[] = ["TagPerformers", "TagAlbum"];
    public filterPropertyValues: Array<IObservable<ObservableArray<string>>> = new Array();

    constructor() {
        // Populate library tree observables
        for (var property in this.filterPropertyNames) {
            this.filterPropertyValues.push(new Observable<ObservableArray<string>>(new ObservableArray<string>()));
        }
    }

    private _loginResponse: IObservable<ILoginResponse> = new Observable<ILoginResponse>();
    public get loginResponse(): IObservable<ILoginResponse> {
        return this._loginResponse;
    }

    public fetchFilterPropertyValues(index: number, filters?: string[]): IObservable<ObservableArray<string>> {
        // Start grabbing new values from the server
        RestRequest.jsonRequest<string[]>(DataModel.TrackPropertiesEndpoint, RestRequestMethod.Post, {
            propertyName: this.filterPropertyNames[index],
            filters: filters
        }).then(
            // Success
            (value: string[]) => {
                this.filterPropertyValues[index].value = new ObservableArray<string>(value);
            },
            // Failure
            (reason) => {
                // TODO
            }
        );

        // Return observable
        return this.filterPropertyValues[index];
    }

    public fetchTracks(filters?: string[]): IObservable<ObservableArray<ITrackModel>> {
        var response = new Observable<ObservableArray<ITrackModel>>(new ObservableArray<ITrackModel>());
        RestRequest.jsonRequest<ITrackModel[]>(DataModel.TracksEndpoint, RestRequestMethod.Post, filters).then(
            // Success
            (value: ITrackModel[]) => {
                response.value = new ObservableArray(value);
            },
            // Failure
            (reason) => {
                // TODO
            }
        );

        return response;
    }
}
