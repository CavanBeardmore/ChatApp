interface MessageProps {
    title: string;
    text: string;
}

export const MessageBox = ({
    title,
    text
}: MessageProps) => {
    return (
        <div
            className="bg-[#1a1a1a] flex flex-col text-left p-4 rounded-lg space-y-6"
        >
            <p className="text-xs font-semibold">{title}</p>
            <p className="text-lg font-thin">{text}</p>
        </div>
    )
}