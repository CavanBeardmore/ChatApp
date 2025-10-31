import { EventObserver } from "./EventObserver";

export enum Events {
    ERROR = "ERROR",

    MESSAGE = "MESSAGE",
    ACTION = "ACTION",

    START_HOST_ERROR = "START_HOST_ERROR",
    STOP_HOST_ERROR = "STOP_HOST_ERROR",

    START_CLIENT_ERROR = "START_CLIENT_ERROR",
    STOP_CLIENT_ERROR = "STOP_CLIENT_ERROR",

    SEND_MESSAGE_ERROR = "SEND_MESSAGE_ERROR", 
    SENT_MESSAGE = "SENT_MESSAGE",
    
    HOST_LEFT = "HOST_LEFT"
}

export class GlobalEventObserver extends EventObserver{}