import { useLocation, useNavigate } from "react-router-dom"
import { useMessages } from "../hooks/useMessages";
import { useEffect, useState, type ChangeEvent } from "react";
import { MessagingFooter } from "../components/messaging-footer";
import { Action } from "../Data/Action";
import { Message } from "../Data/Message";
import { MessagingWindow } from "../components/messaging-window";
import { useClientService } from "../hooks/useClientService";

export const Client = () => {

    const navigate  = useNavigate();
    const location = useLocation();

    const username: string = location.state.username;

    const {
        isConnected,
        startClient,
        stopClient,
        sendMessage,
        registerClientEvents,
        removeClientEvents
    } = useClientService(username);

    const {
        getMessages,
        registerMessageEvents,
        removeMessageEvents,
    } = useMessages();

    const messages: (Action | Message)[] = getMessages();

    const [inputtedMessage, setInputtedMessage] = useState<string | null>(null);
    const [hasLeftChat, setHasLeftChat] = useState<boolean>(false);

    const handleJoinChat = async (): Promise<void> => {
        if (hasLeftChat) {
            setHasLeftChat(false);
        }
        await startClient();
    }

    const handleLeaveChat = async (): Promise<void> => {
        setHasLeftChat(true);
        await stopClient();
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
        registerClientEvents();

        return () => {
            removeMessageEvents();
            removeClientEvents();
        }
    }, []);

    return (
        <div className='flex flex-col space-y-6 text-green-500 w-full h-screen'>
            <MessagingWindow messages={messages} />
             <div className="fixed bottom-0 left-0 right-0 px-10 pb-2 pt-8 bg-[#1a1a1a] shadow-md space-y-4">
                <MessagingFooter 
                    inputtedMessage={inputtedMessage || ""} 
                    isButtonDisabled={!inputtedMessage?.trim() || !isConnected}  
                    onSendMessageClicked={onSendMessageClicked}
                    onMessageInputChange={handleMessageInput}
                />
                <div className="flex flex-row space-x-4 w-full justify-evenly">
                    <button className="bg-[#242424]" disabled={isConnected} onClick={handleJoinChat}>Join</button>
                    {!isConnected && (
                            <button className="bg-[#242424]" disabled={!isConnected} onClick={handleLeaveChat}>Leave</button>
                        )
                    }
                    {isConnected && (
                            <button className="bg-[#242424]" onClick={handleQuit}>Quit</button>
                        )
                    }
                </div>
            </div>
        </div>
    )
}

