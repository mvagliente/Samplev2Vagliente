import restClient from "./RestClient";

export class Office {
    constructor(public id: string, public address: string) {}
}

export class OfficeService {
    public getOffices(pattern: string) {
        return restClient.get<Office[]>("offices/getOffices", { searchPattern: pattern });
    }
}


let officeService = new OfficeService()
export default officeService