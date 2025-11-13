import { ActionType } from "../Data/Action";

interface MessageProps {
    type: ActionType;
    username: string;
    timestamp: string;
}

export const ActionBox = ({
    type,
    username,
    timestamp
}: MessageProps) => {

    const generateText = (): string => {
        if (type === ActionType.JOINED) {
            `You have joined the chat at - ${timestamp}`
        }

        return `
            ${username} has ${type === ActionType.JOIN ? "joined" : "left"} the chat at ${timestamp}
        `;
    }

    return (
        <div
            className="bg-[#1a1a1a] flex flex-col text-center p-4 rounded-lg border-1 border-s-green-500"
        >
            <p>
                {generateText()}
            </p>
        </div>
    )
}