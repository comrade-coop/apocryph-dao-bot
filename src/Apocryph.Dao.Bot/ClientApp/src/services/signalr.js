import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";

const signalR = {
    url: null,
    connection: null,
    view: null,

    connect(baseUrl, session) {
        this.connection = new HubConnectionBuilder()
            .withUrl(`${baseUrl}/ws?session=${session}`)
            .configureLogging(LogLevel.Debug)
            .build();

        this.connection.onclose(() => this.connection.start());
        this.connection.start();
    }
}

export default signalR;