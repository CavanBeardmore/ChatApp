import { useState, type ChangeEvent } from "react";
import { useNavigate } from "react-router-dom"

export const Home = () => {

    const navigate = useNavigate();

    const [username, setUsername] = useState<string | null>(null);

    const isUsernameNull: boolean = username === null;

    const handleUsernameInput = (e: ChangeEvent<HTMLInputElement>) => {
        const { value } = e.target;

        setUsername(value);
    }

    const onHostClicked = async () => {
        await navigate("/host", {state: { username }});
    };

    const onClientClicked = async () => {
        await navigate("/client", {state: { username }});
    };

    return (
        <div className='flex flex-col space-y-6 text-green-500 w-full'>
            <p className='text-green-500'>Enter a username and choose a chat option.</p>
            <input placeholder="Username" value={username || ""} onChange={handleUsernameInput} />
            <button disabled={isUsernameNull} onClick={onHostClicked}>
                Host a chat.
            </button>
            <button disabled={isUsernameNull} onClick={onClientClicked}>
                Join a chat.
            </button>
        </div>
    )
}