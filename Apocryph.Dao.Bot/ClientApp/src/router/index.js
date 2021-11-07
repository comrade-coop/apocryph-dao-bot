import { createWebHistory, createRouter } from "vue-router";
import Introduction from "@/components/Introduction.vue";

const routes = [
    {
        path: "/introduction/:session/:message",
        name: "Introduction",
        component: Introduction,
    } 
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

export default router;