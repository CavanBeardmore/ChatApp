export class Message {
    constructor(
        public readonly Sender: Readonly<string>,
        public readonly Timestamp: Readonly<string>,
        public readonly Text: Readonly<string>
    ){}
}