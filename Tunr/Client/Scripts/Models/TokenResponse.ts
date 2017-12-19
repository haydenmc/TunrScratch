interface TokenResponse {
    authenticated: boolean;
    entityId: string;
    token: string;
    tokenExpires: Date;
}
