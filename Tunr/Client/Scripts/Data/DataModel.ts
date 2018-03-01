import { Observable } from "./Observable";
import { IUserModel } from "./Models/IUserModel";
import { IObservable } from "./IObservable";

export class DataModel {
    private _user: IObservable<IUserModel> = new Observable<IUserModel>();
    public get user(): IObservable<IUserModel> {
        return this._user;
    }
}
