import { useLocation, useNavigate } from "react-router-dom"
import { useHostService } from "../hooks/useHostService";
import { useMessages } from "../hooks/useMessages";
import { useEffect, useState, type ChangeEvent } from "react";
import { MessagingFooter } from "../components/messaging-footer";
import { Action } from "../Data/Action";
import { Message } from "../Data/Message";
import { MessagingWindow } from "../components/messaging-window";

export const Host = () => {

    const navigate  = useNavigate();
    const location = useLocation();

    const username: string = location.state.username;

    const {
        isHosting,
        startHost,
        stopHost,
        sendMessage
    } = useHostService(username);

    const {
        getMessages,
        registerMessageEvents,
        removeMessageEvents
    } = useMessages();

    const messages: (Action | Message)[] = getMessages();

    const [inputtedMessage, setInputtedMessage] = useState<string | null>(null);
    const [hasStoppedHosting, setHasStoppedHosting] = useState<boolean>(false);

    const handleStartingHost = async (): Promise<void> => {
        if (hasStoppedHosting) {
            setHasStoppedHosting(false);
        }
        await startHost();
    }

    const handleStoppingHost = async (): Promise<void> => {
        setHasStoppedHosting(true);
        await stopHost();
    }

    const handleQuit = async (): Promise<void> => {
        navigate("/");
    }

    const handleMessageInput = (e: ChangeEvent<HTMLInputElement>) => {
        const trimmedValue: string = e.target.value;
        setInputtedMessage(trimmedValue);
    }

    const onSendMessageClicked = async () => {
        if (inputtedMessage) {
            const val = inputtedMessage;
            setInputtedMessage(null);
            await sendMessage(val);
        }
    }

    useEffect(() => {
        registerMessageEvents();

        return () => {
            removeMessageEvents();
        }
    }, []);

    return (
        <div className='flex flex-col space-y-6 text-green-500 w-full h-screen'>
            <MessagingWindow messages={messages} />
             <div className="fixed bottom-0 left-0 right-0 px-10 pb-2 pt-8 bg-[#1a1a1a] shadow-md space-y-4">
                <MessagingFooter 
                    inputtedMessage={inputtedMessage || ""} 
                    isButtonDisabled={!inputtedMessage?.trim() || !isHosting}  
                    onSendMessageClicked={onSendMessageClicked}
                    onMessageInputChange={handleMessageInput}
                />
                <div className="flex flex-row space-x-4 w-full justify-evenly">
                    <button className="bg-[#242424]" disabled={isHosting} onClick={handleStartingHost}>Start Hosting</button>
                    {!hasStoppedHosting && (
                            <button className="bg-[#242424]" disabled={!isHosting} onClick={handleStoppingHost}>Stop Hosting</button>
                        )
                    }
                    {hasStoppedHosting && (
                            <button className="bg-[#242424]" onClick={handleQuit}>Quit</button>
                        )
                    }
                </div>
            </div>
        </div>
    )
}

