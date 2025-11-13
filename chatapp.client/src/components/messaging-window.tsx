import { List, type RowComponentProps, useDynamicRowHeight, } from "react-window";
import { Action } from "../Data/Action";
import { Message } from "../Data/Message";
import { MessageBox } from "./message-box";
import { ActionBox } from "./action-box";

interface MessagingWindowProps {
    messages: (Message | Action)[]
}

export const MessagingWindow = ({
    messages
}: MessagingWindowProps) => {

    const rowHeight = useDynamicRowHeight({
        defaultRowHeight: 200
    })

    const ListItem = ({messages, index, style }: RowComponentProps<{messages: (Action | Message)[]}>) => {

        const render = () => {
            const item = messages[index];
            if (item instanceof Message) {
                const {Sender, Timestamp, Text} = item;
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
                    key={`${item.Timestamp} - ${item.Username}`}
                    type={item.Type}
                    username={item.Username}
                    timestamp={item.Timestamp}
                />
            )
        }

        return (
            <div style={style}>
                {render()}
            </div>
        )
    }

    return (
        <div
            style={{ height: "calc(100vh - 16rem)" }}
        >
            <List
                rowComponent={ListItem}
                rowCount={messages.length}
                rowHeight={rowHeight}
                rowProps={{ messages }}
            />
        </div>
    )
}