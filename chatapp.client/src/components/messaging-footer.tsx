import type { ChangeEvent } from "react";

interface MessagingFooterProps {
    inputtedMessage: string;
    isButtonDisabled: boolean;
    onMessageInputChange: (e: ChangeEvent<HTMLInputElement>) => void;
    onSendMessageClicked: () => void;
}

export const MessagingFooter = ({
    inputtedMessage,
    isButtonDisabled,
    onMessageInputChange,
    onSendMessageClicked
}: MessagingFooterProps) => {
    return (
        <div className="flex flex-col space-y-6 text-green-500 w-full">
            <input className="bg-[#242424]" type="text" onChange={onMessageInputChange} value={inputtedMessage} placeholder="Type message here..."/>
            <button className="bg-[#242424]" disabled={isButtonDisabled} onClick={onSendMessageClicked}>Send Message</button>
        </div>
    )
}