import { Message } from "../../Data/Message";
import { RequestMethod, type IHttpService } from "../Http/IHttpService";
import type { IServiceResponse } from "../IServiceResponses";
import type { IClientService } from "./IClientService";

export class ClientService implements IClientService {
    constructor(
        private readonly _httpService: IHttpService,
        private readonly _clientUrl: string
    ) {}

    public async StartClient(username: string): Promise<boolean> {
        const body = JSON.stringify({
            username
        });

        const res = await this._httpService.Request(
            `${this._clientUrl}/start-client`,
            {
                method: RequestMethod.POST,
                body,
                headers: [
                    {
                        key: "Content-Type",
                        value: "application/json"
                    }
                ]
            },
        );

        console.log("ClientService - StartClient - res", res);

        return res.success;
    }

    public async StopClient(): Promise<boolean> {
        const res = await this._httpService.Request(
            `${this._clientUrl}/stop-client`,
            {
                method: RequestMethod.POST
            }
        );

        console.log("ClientService - StopClient - res", res);

        return res.success;
    }

    public async SendMessage(username: string, message: string): Promise<IServiceResponse<Message>> {
        const body = JSON.stringify({
            username,
            message
        });

        const res = await this._httpService.Request<Message>(
            `${this._clientUrl}/send-message-client`,
            {
                method: RequestMethod.POST,
                body,
                headers: [
                    {
                        key: "Content-Type",
                        value: "application/json"
                    }
                ]
            }
        );

        console.log("ClientService - SendMessage - res", res);

        const {success, data} = res;

        if (success) {
            return {success, data: new Message(data.Sender, data.Timestamp, data.Text)}
        }

        return { success };
    }
}