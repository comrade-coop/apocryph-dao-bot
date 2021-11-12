import {HubConnectionBuilder, LogLevel} from "@microsoft/signalr";

const SignalRService = {
    url: null,
    connection: null,
    view: null,

    connect(baseUrl, session) {
        this.connection = new HubConnectionBuilder()
            .withUrl(`${baseUrl}/ws?session=${session}`)
            .configureLogging(LogLevel.Debug)
            .build();

        this.connection.onclose(() => this.connection.start());
        this.connection.on("onError", (args) => {
            this.view.$emit("on-error", args);
        });
        
        this.connection.start();
    },
    
    subscribe(view, eventName) {
        this.connection.on(eventName, (args) => {
            view.$emit(eventName, args);    
        });
    }
}

export default SignalRService;