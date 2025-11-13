import { Resolve } from "@here-mobility/micro-di";
import { Events, type GlobalEventObserver } from "../classes/Events/GlobalEventObserver";
import { useState } from "react";
import { Action, ActionType } from "../Data/Action";
import { Message } from "../Data/Message";

interface useMessagesReturn {
    getMessages(): (Action | Message)[];
    removeMessageEvents(): void;
    registerMessageEvents(): void;
}

export const useMessages = (): useMessagesReturn => {

    const events: GlobalEventObserver = Resolve<GlobalEventObserver>("GlobalEventObserver");

    const [messages, setMessages] = useState<(Action | Message)[]>([]);
    
    //@ts-ignore
    window.messages = messages;

    const getMessages = (): (Action | Message)[] => {
        return messages;
    }

    const registerMessageEvents = (): void => {
        events.add(
            Events.MESSAGE, 
            `${Events.MESSAGE}_MESSAGE_HOOKS_ID`,
            (val: any) => {
                const message = new Message(val.Sender, val.Timestamp, val.Text);

                setMessages((prev) => {
                    return [...prev, message];
                });
            }
        );

        events.add(
            Events.SENT_MESSAGE, 
            `${Events.SENT_MESSAGE}_MESSAGE_HOOKS_ID`,
            (val: any) => {
                const message = new Message(val.Sender, val.Timestamp, val.Text);

                setMessages((prev) => {
                    return [...prev, message];
                });
            }
        );

        events.add(
            Events.ACTION,
            `${Events.ACTION}_MESSAGE_HOOKS_ID`,
            (val: any) => {
                const action = new Action(val.Type, val.Username, val.Timestamp);
                setMessages((prev) => {
                    return [...prev, action];
                });
            }
        );
    }

    const removeMessageEvents = (): void => {
        events.remove(
            Events.MESSAGE, 
            `${Events.MESSAGE}_MESSAGE_HOOKS_ID`,
        );

        events.remove(
            Events.SENT_MESSAGE, 
            `${Events.SENT_MESSAGE}_MESSAGE_HOOKS_ID`
        );

        events.remove(
            Events.ACTION,
            `${Events.ACTION}_MESSAGE_HOOKS_ID`,
        );
    }

    return {
        getMessages,
        registerMessageEvents,
        removeMessageEvents
    }
}