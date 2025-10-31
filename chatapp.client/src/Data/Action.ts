export enum ActionType {
    JOIN,
    LEAVE,
    JOINED
}

export class Action {
    constructor(
        public readonly Type: Readonly<ActionType>,
        public readonly Username: Readonly<string>,
        public readonly Timestamp: Readonly<string>
    ){}
}