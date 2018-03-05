import { IObservable } from "./IObservable";
import { Observable } from "./Observable";
import { ILoginResponse } from "./Models/ILoginResponse";

export class DataModel {
    private _loginResponse: IObservable<ILoginResponse> = new Observable<ILoginResponse>();
    public get loginResponse(): IObservable<ILoginResponse> {
        return this._loginResponse;
    }
}
