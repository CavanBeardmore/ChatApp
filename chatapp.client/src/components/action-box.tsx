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
            `You joined the chat at ${timestamp}`
        }

        return `
            ${username} ${type === ActionType.JOIN ? "joined" : "left"} at ${timestamp}
        `;
    }

    return (
        <div
            className="bg-[#1a1a1a] flex flex-col text-left p-4 rounded-lg space-y-6"
        >
            <p className="text-xs font-semibold">
                {generateText()}
            </p>
        </div>
    )
}