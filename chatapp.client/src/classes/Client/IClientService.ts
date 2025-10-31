import type { Message } from "../../Data/Message";
import type { IServiceResponse } from "../IServiceResponses";

export interface IClientService {
    StartClient(username: string): Promise<boolean>;
    StopClient(): Promise<boolean>;
    SendMessage(username: string, message: string): Promise<IServiceResponse<Message>>; 
}