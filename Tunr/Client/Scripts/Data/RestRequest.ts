export class RestRequest {
    public static jsonRequest<T>(url: string, method: RestRequestMethod, data?: any): Promise<T> {
        return new Promise<T>((resolve, reject) => {
            let requestHeaders = new Headers();
            let requestBody: string;
            let requestMethodString: string;

            // Calculate method-specific request properties
            switch (method) {
                case RestRequestMethod.Get:
                    requestMethodString = "GET";
                    if (data != null) {
                        url += this.createQueryString(data);
                    }
                    break;
                case RestRequestMethod.Post:
                    requestMethodString = "POST";
                    requestBody = JSON.stringify(data);
                    requestHeaders.append("Content-Length", requestBody.length.toString());
                    requestHeaders.append("Content-Type", "application/json");
                    break;
                default:
                    reject("Unsupported request method.");
                    return;
            }

            // Make request
            fetch(url, {
                method: requestMethodString,
                headers: requestHeaders,
                body: requestBody,
                credentials: "same-origin"
            }).then(
                // Fetch success
                (response) => {
                    if (response.ok) {
                        // Server responds with success
                        response.json().then(
                            // JSON parse success
                            (value: T) => {
                                resolve(value);
                            },
                            // JSON parse failure
                            (reason) => {
                                reject(reason);
                            }
                        )
                    } else {
                        // Server responds with error
                        reject("Server responded with error " + response.status + ": " + response.statusText);
                    }
                },
                // Fetch internal failure
                (reason) => {
                    reject(reason);
                }
            );
        });
    }

    public static createQueryString(data: any): string {
        let queryString = "?";
        for (var prop in data) {
            if (queryString.length > 1) {
                queryString += "&";
            }
            queryString += encodeURIComponent(prop) + "=" + encodeURIComponent(data[prop]);
        }
        return queryString;
    }
}

export enum RestRequestMethod {
    Get,
    Post
}
