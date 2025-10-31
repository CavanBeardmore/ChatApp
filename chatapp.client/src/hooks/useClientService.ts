import { useRef, useState } from "react";
import { Resolve } from "@here-mobility/micro-di";
import { Events, type GlobalEventObserver } from "../classes/Events/GlobalEventObserver";
import type { IClientService } from "../classes/Client/IClientService";
import { ActionType } from "../Data/Action";

interface useClientServiceReturn {
    isConnected: boolean;
    startClient(): Promise<void>;
    stopClient(): Promise<void>;
    sendMessage(message: string): Promise<void>;
    registerClientEvents(): void;
    removeClientEvents(): void;
}

const DEFAULT_HOST_NAME = "HOST";

export const useClientService = (uname: string): useClientServiceReturn => {

    const clientUsername = useRef<string>(uname);
    const clientService: IClientService = Resolve<IClientService>("ClientService");
    const events: GlobalEventObserver =  Resolve<GlobalEventObserver>("GlobalEventObserver");

    const [isConnected, setIsConnected] = useState<boolean>(false);

    const registerClientEvents = (): void => {
        events.add(Events.ACTION, `${Events.ACTION}_CLIENT_HOOK_ID`, (val: any) => {
            if (val?.Type === ActionType.JOINED) {
                setIsConnected(true);
            }

            if (val?.Type === ActionType.LEAVE && val?.Username === DEFAULT_HOST_NAME) {
                events.raise(Events.HOST_LEFT);
                setIsConnected(false);
            }
        });
    }

    const removeClientEvents = (): void => {
        events.remove(Events.ACTION, `${Events.ACTION}_CLIENT_HOOK_ID`);
    }

    const startClient = async (): Promise<void> => {
        const success = await clientService.StartClient(clientUsername.current);

        if (!success) {
            events.raise(Events.START_CLIENT_ERROR);
        }
    };

    const stopClient = async (): Promise<void> => {
        const success = await clientService.StopClient();
        setIsConnected(!success);

        if (!success) {
            events.raise(Events.STOP_CLIENT_ERROR);
        }
    };

    const sendMessage = async (message: string): Promise<void> => {
        const username = clientUsername.current;
        const {success, data} = await clientService.SendMessage(username, message);

        if (!success) {
            events.raise(Events.SEND_MESSAGE_ERROR);
        } else {
            events.raise(Events.SENT_MESSAGE, data)
        }
    };

    return {
        isConnected,
        startClient,
        stopClient,
        sendMessage,
        registerClientEvents,
        removeClientEvents
    }
}