import { createWebHistory, createRouter } from "vue-router";
import SignAddress from "@/components/SignAddress.vue";

const routes = [
    {
        path: "/sign-address/:session/:message",
        name: "SignAddress",
        component: SignAddress,
    } 
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;