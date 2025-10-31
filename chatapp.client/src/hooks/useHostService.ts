import { useRef, useState } from "react";
import type { IHostService } from "../classes/Host/IHostService";
import { Resolve } from "@here-mobility/micro-di";
import { Events, type GlobalEventObserver } from "../classes/Events/GlobalEventObserver";

interface useHostServiceReturn {
    isHosting: boolean;
    startHost(): Promise<void>;
    stopHost(): Promise<void>;
    sendMessage(message: string): Promise<void>;
}

export const useHostService = (uname: string): useHostServiceReturn => {

    const hostUsername = useRef<string>(uname);
    const hostService: IHostService = Resolve<IHostService>("HostService");
    const events: GlobalEventObserver =  Resolve<GlobalEventObserver>("GlobalEventObserver");

    const [isHosting, setIsHosting] = useState<boolean>(false);

    const startHost = async (): Promise<void> => {
        const success = await hostService.StartHost();
        setIsHosting(success);

        if (!success) {
            events.raise(Events.START_HOST_ERROR);
        }
    };

    const stopHost = async (): Promise<void> => {
        const success = await hostService.StopHost();
        setIsHosting(!success);
        if (!success) {
            events.raise(Events.STOP_HOST_ERROR);
        }
    };

    const sendMessage = async (message: string): Promise<void> => {
        const username = hostUsername.current;
        const [success, sentMessage] = await hostService.SendMessage(username, message);

        if (!success) {
            events.raise(Events.SEND_MESSAGE_ERROR);
        } else {
            events.raise(Events.SENT_MESSAGE, sentMessage)
        }
    };

    return {
        isHosting,
        startHost,
        stopHost,
        sendMessage
    }
}