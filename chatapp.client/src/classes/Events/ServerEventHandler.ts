import { EventObserver } from "./EventObserver";
import { Events } from "./GlobalEventObserver";
import type { IServerEventHandler } from "./IServerEventHandler";

interface ServerEvent {
    Type: Events;
    Data?: unknown
}

export class ServerEventHandler implements IServerEventHandler {
    private _eventSource?: EventSource;

    constructor(
        private readonly _backendUrl: string,
        private readonly _eventObserver: EventObserver
    ) {}

    public async CreateConnection(): Promise<void> {
        this._eventSource = new EventSource(`${this._backendUrl}/Event`);

        this._eventSource.onmessage = (event: any) => this.OnMessage(event);
        this._eventSource.onerror = () => this.OnError();
        this._eventSource.onopen = () => {
            console.log("Connection opened.");
        };
    }

    public CloseConnection(): void {
        this._eventSource?.close();
    }

    private OnMessage(e: MessageEvent): void {
        console.log("ServerEventHandler - OnMessage - received event", e);
        const event: ServerEvent= JSON.parse(e.data);
        const {Type: eventType, Data} = event;

        this.raiseEvent(eventType, Data);
    }

    private OnError(): void {
        this._eventObserver.raise(Events.ERROR);
    }

    private raiseEvent(eventType: Events, data: unknown) {
        this._eventObserver.raise(eventType, data);
    }
}