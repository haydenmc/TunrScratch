import { observable } from "mobx";
import { LoginStore } from "./LoginStore";

export class ApplicationStore {
    @observable
    public loginStore: LoginStore = new LoginStore();
}