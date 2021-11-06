import { createWebHistory, createRouter } from "vue-router";
import Introduction from "@/components/Introduction.vue";
import Counter from "@/components/Counter.vue";
import FetchData from "@/components/FetchData.vue";

const routes = [
    {
        path: "/introduction/:message",
        name: "Introduction",
        component: Introduction,
    },
    {
        path: "/Counter",
        name: "Counter",
        component: Counter,
    },
    {
        path: "/FetchData",
        name: "FetchData",
        component: FetchData,
    }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;