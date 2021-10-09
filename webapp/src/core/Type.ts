import { Prop } from "@vue/runtime-core";


export default function type<T>() : Prop<T> {
    return null as unknown as Prop<T>
}