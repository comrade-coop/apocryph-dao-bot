import { createWebHistory, createRouter } from "vue-router";
import SignAddress from "@/components/SignAddress.vue";
import Vote from "@/components/Vote.vue";
import VoteCreate from "@/components/VoteCreate.vue";


const routes = [
    {
        path: "/sign-address/:session/:address",
        name: "SignAddress",
        component: SignAddress,
    },
    {
        path: "/vote/create",
        name: "VoteCreate",
        component: VoteCreate
    },
    { 
        path: "/vote/:voteId/:cid",
        name: "Vote",
        component: Vote,
        props: {
            voteId:{
                type: [Number]
            },
            cid: {
                type: [String],
                default: ""
            }
        }
    }
];
 
const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;