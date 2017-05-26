import * as React from "react";
import { observer } from "mobx-react";
import { ApplicationStore } from "../data/ApplicationStore";
import { LoginStore } from "../data/LoginStore";

// Locally scoped styles
var styles = {
    outerContainer: {
        display: "flex",
        justifyContent: "center",
        alignItems: "center",
        width: "100%",
        height: "100%"
    } as React.CSSProperties,

    innerContainer: {
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
        width: "100%",
        maxWidth: 280
    } as React.CSSProperties,

    form: {
        width: "100%",
        display: "flex",
        flexDirection: "row"
    } as React.CSSProperties,

    formInputContainer: {
        width: "100%",
        display: "flex",
        flexShrink: 1,
        flexDirection: "column"
    } as React.CSSProperties,

    formActionContainer: {
        width: "48px",
        display: "flex",
        flexDirection: "column",
        justifyContent: "flex-end",
        transition: "width 200ms ease-out"
    } as React.CSSProperties,

    input: {
        width: "100%",
        boxSizing: "border-box",
        paddingTop: 8,
        paddingBottom: 8,
        border: "none"
    } as React.CSSProperties,

    submit: {
        width: "40px",
        height: "40px",
        fontSize: "20px",
        boxSizing: "border-box",
        backgroundColor: "black",
        border: "none",
        color: "white",
        fontFamily: "var(--font-family)",
        fontWeight: "bold",
        textTransform: "uppercase"
    } as React.CSSProperties
};

interface LoginProps {
    applicationStore: ApplicationStore;
}

@observer
export class Login extends React.Component<LoginProps, undefined> {
    private loginStore: LoginStore;

    constructor(props: LoginProps) {
        super(props);
        this.loginStore = props.applicationStore.loginStore;
    }

    render() {
        return (
            <div style={styles.outerContainer}>
                <div style={styles.innerContainer}>
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 3072 1025">
                        <title>Tunr Logo</title>
                        <g id="Logo_Type_1" data-name="Logo Type #1">
                            <rect x="380.5" y="432.5" width="255" height="159"/>
                            <path d="M635,433V591H381V433H635m1-1H380V592H636V432Z"/>
                            <rect x="380.5" y="616.5" width="255" height="159"/>
                            <path d="M635,617V775H381V617H635m1-1H380V776H636V616Z"/>
                            <rect x="100.5" y="616.5" width="255" height="159"/>
                            <path d="M355,617V775H101V617H355m1-1H100V776H356V616Z"/>
                            <rect x="660.5" y="616.5" width="255" height="159"/>
                            <path d="M915,617V775H661V617H915m1-1H660V776H916V616Z"/>
                            <rect x="660.5" y="432.5" width="255" height="159"/>
                            <path d="M915,433V591H661V433H915m1-1H660V592H916V432Z"/>
                            <rect x="660.5" y="248.5" width="255" height="159"/>
                            <path d="M915,249V407H661V249H915m1-1H660V408H916V248Z"/>
                            <path d="M1231.28,768.63H1090.89V363.8H964V249h393.81V363.8H1231.28Z"/>
                            <path d="M1853.51,249V561.78q0,102-57.76,158t-166.52,56q-106.28,0-163.32-54.38t-57-156.39V249H1550V554q0,55.1,20.61,80t60.78,24.88q43,0,62.38-24.7t19.37-80.86V249Z"/>
                            <path d="M2437,768.63H2252.89l-189.8-366.09h-3.2q6.75,86.37,6.75,131.86V768.63h-124.4V249h183.4l189.09,361.11h2.13q-5-78.55-5-126.18V249H2437Z"/>
                            <path d="M2669.33,579.55V768.63H2528.93V249h170.25Q2911,249,2911,402.54q0,90.28-88.15,139.68l151.41,226.41H2815.05L2704.87,579.55Zm0-105.56h26.3q73.57,0,73.57-65,0-53.66-72.15-53.67h-27.72Z"/>
                        </g>
                    </svg>

                    { /* Loading Indicator */ }
                    { this.loginStore.isBusy &&
                        <div>Loading . . .</div> }

                    { /* Error information */ }
                    { this.loginStore.errorText !== "" &&
                        <div>{this.loginStore.errorText}</div> }

                    { /* Login Form */ }
                    <form style={styles.form} onSubmit={(e) => { e.preventDefault(); this.loginStore.processLogin(); }}>
                        <div style={styles.formInputContainer}>
                            <input
                                name="email"
                                type="email"
                                placeholder="e-mail address"
                                value={this.loginStore.email}
                                onChange={(e) => this.loginStore.email = (e.target as HTMLInputElement).value}
                                disabled={this.loginStore.isBusy}
                                style={styles.input}
                                autoFocus />
                            <input
                                name="password"
                                type="password"
                                placeholder="password"
                                value={this.loginStore.password}
                                onChange={(e) => this.loginStore.password = (e.target as HTMLInputElement).value}
                                disabled={this.loginStore.isBusy}
                                style={styles.input} />
                        </div>
                        <div style={styles.formActionContainer}>
                            <input
                                type="submit"
                                value="⏎" 
                                style={styles.submit} />
                        </div>
                    </form>

                    { /* Registration Form */ }
                    { this.loginStore.doesLoginEmailBelongToAccount === false && 
                        <form style={styles.form}>
                            <input
                                name="email"
                                type="email"
                                placeholder="e-mail address"
                                value={this.loginStore.email}
                                onChange={(e) => this.loginStore.email = (e.target as HTMLInputElement).value}
                                style={styles.input} />
                            <input
                                name="password"
                                type="password"
                                placeholder="password"
                                value={this.loginStore.password}
                                onChange={(e) => this.loginStore.password = (e.target as HTMLInputElement).value}
                                style={styles.input} 
                                autoFocus />
                            <input
                                type="submit"
                                value="Register"
                                style={styles.submit} />
                        </form> }
                </div>
            </div>
        );
    }
}