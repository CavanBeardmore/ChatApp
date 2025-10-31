import { Message } from "../../Data/Message";
import { RequestMethod, type IHttpService } from "../Http/IHttpService";
import type { IServiceResponse } from "../IServiceResponses";
import type { IHostService } from "./IHostService";

export class HostService implements IHostService {
    constructor(
        private readonly _httpService: IHttpService,
        private readonly _hostUrl: string
    ) {}

    public async StartHost(): Promise<boolean> {
        const res = await this._httpService.Request(
            `${this._hostUrl}/start-host`,
            {
                method: RequestMethod.POST
            }
        );

        console.log("HostService - StartHost - res", res);

        return res.success;
    }

    public async StopHost(): Promise<boolean> {
        const res = await this._httpService.Request(
            `${this._hostUrl}/stop-host`,
            {
                method: RequestMethod.POST
            }
        );

        console.log("HostService - StopHost - res", res);

        return res.success;
    }

    public async SendMessage(username: string, message: string): Promise<IServiceResponse<Message>> {
        const body = JSON.stringify({
            username,
            message
        });

        const res = await this._httpService.Request<Message>(
            `${this._hostUrl}/send-message-host`,
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

        console.log("HostService - SendMessage - res", res);

        const {success, data} = res;

        if (success) {
            return {success, data: new Message(data.Sender, data.Timestamp, data.Text)};
        }

        return {success};
    }
}