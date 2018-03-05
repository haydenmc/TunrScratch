import { IObservable } from "../IObservable";
import { Observable } from "../Observable";

export class DataModel {
    private _loginResponse: IObservable<LoginResponse> = new Observable<LoginResponse>();
    public get loginResponse(): IObservable<LoginResponse> {
        return this._loginResponse;
    }
}
