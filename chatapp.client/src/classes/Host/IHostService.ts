import type { Message } from "../../Data/Message";
import type { IServiceResponse } from "../IServiceResponses";

export interface IHostService {
    StartHost(): Promise<boolean>;
    StopHost(): Promise<boolean>;
    SendMessage(username: string, message: string): Promise<IServiceResponse<Message>>;
}