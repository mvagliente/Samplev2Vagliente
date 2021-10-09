import { Office } from "./OfficeService";
import restClient from "./RestClient";

export class Role {
    constructor(public id: string, public name: string) {}
}

export class UserRole {
    constructor(public id: string, public role: Role) {}
}

export class User {
    constructor(
        public id: string,
        public office: Office, 
        public login: string, 
        public roles: UserRole[]) {}
}

export class UserService {
    public getUsers(officeIds: string[]): any {
        let ids = ''
        officeIds.forEach(element => {
            ids += element + ','
        });

        restClient.get<User[]>("users/getUsers", {officeIds: ids})
    }
}

let userService = new UserService()
export default userService