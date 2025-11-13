import { Resolve } from "@here-mobility/micro-di";
import { Events, type GlobalEventObserver } from "../classes/Events/GlobalEventObserver";
import { toast } from "react-toastify";

interface useToastErrorsReturn {
    registerEvents: () => void;
    removeEvents: () => void;
}

export const useToastErrors = (): useToastErrorsReturn => {

    const events: GlobalEventObserver =  Resolve<GlobalEventObserver>("GlobalEventObserver");
    
    const registerEvents = (): void => {
        events.add(
            Events.START_HOST_ERROR, 
            `${Events.START_HOST_ERROR}_TOAST_EVENTS_HOOKS_ID`,
            () => {
                toast("Failed to start hosting.");
            }
        );

        events.add(
            Events.STOP_HOST_ERROR, 
            `${Events.STOP_HOST_ERROR}_TOAST_EVENTS_HOOKS_ID`,
            () => {
                toast("Failed to stop hosting.");
            }
        );

        events.add(
            Events.SEND_MESSAGE_ERROR, 
            `${Events.SEND_MESSAGE_ERROR}_TOAST_EVENTS_HOOKS_ID`,
            () => {
                toast("Failed to send message.");
            }
        );

        events.add(
            Events.START_CLIENT_ERROR, 
            `${Events.START_CLIENT_ERROR}_TOAST_EVENTS_HOOKS_ID`,
            () => {
                toast("Failed to join chat as client.");
            }
        );

        events.add(
            Events.STOP_CLIENT_ERROR, 
            `${Events.STOP_CLIENT_ERROR}_TOAST_EVENTS_HOOKS_ID`,
            () => {
                toast("Failed to leave chat as client.");
            }
        );
    }

    const removeEvents = (): void => {
        events.remove(
            Events.START_HOST_ERROR, 
            `${Events.START_HOST_ERROR}_TOAST_EVENTS_HOOKS_ID`,
        );

        events.remove(
            Events.STOP_HOST_ERROR, 
            `${Events.STOP_HOST_ERROR}_TOAST_EVENTS_HOOKS_ID`,
        );

        events.remove(
            Events.SEND_MESSAGE_ERROR, 
            `${Events.SEND_MESSAGE_ERROR}_TOAST_EVENTS_HOOKS_ID`,
        );

        events.remove(
            Events.START_CLIENT_ERROR, 
            `${Events.START_CLIENT_ERROR}_TOAST_EVENTS_HOOKS_ID`,
        );

        events.remove(
            Events.STOP_CLIENT_ERROR, 
            `${Events.STOP_CLIENT_ERROR}_TOAST_EVENTS_HOOKS_ID`,
        );
    }

    return {
        registerEvents,
        removeEvents
    }
}