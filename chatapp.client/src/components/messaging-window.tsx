import { Action } from "../Data/Action";
import { Message } from "../Data/Message";
import { ActionBox } from "./action-box";
import { MessageBox } from "./message-box";

interface MessagingWindowProps {
    messages: (Message | Action)[]
}

export const MessagingWindow = ({
    messages
}: MessagingWindowProps) => {
    return (
        <div
            className="overflow-y-auto p-4 space-y-6"
            style={{ height: "calc(100vh - 16rem)" }}
        >
            {messages.map((value) => {
                if (value instanceof Message) {
                    const {Sender, Timestamp, Text} = value;
                    const title = `${Sender} - ${Timestamp}`;
                        return (
                            <MessageBox
                                key={title}
                                title={title}
                                text={Text} 
                            />
                        )
                }

                return (
                    <ActionBox
                        key={`${value.Timestamp} - ${value.Username}`}
                        type={value.Type}
                        username={value.Username}
                        timestamp={value.Timestamp}
                    />
                )
            })}
        </div>
    )
}