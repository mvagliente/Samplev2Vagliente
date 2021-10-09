import axios from "axios"

const serverUrl = "http://localhost:5000/"
export class RestClient {
    public get<T>(url: string, params: any) {
        return axios.get<T>(serverUrl + url, {params})
                .then(o => o.data)
    }
}

let restClient = new RestClient()

export default restClient