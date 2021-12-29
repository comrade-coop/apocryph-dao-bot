import { createWebHistory, createRouter } from "vue-router";
import SignAddress from "@/components/SignAddress.vue";
import Vote from "@/components/Vote.vue";

const routes = [
    {
        path: "/sign-address/:session/:address",
        name: "SignAddress",
        component: SignAddress,
    },
    { 
        path: "/vote/:session/:voteId",
        name: "Vote",
        component: Vote,
        props: {
            voteId: {
                type: [Number],
                default: 0
            },
            session: {
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