import http from "k6/http";
import { check, sleep } from "k6";

export const options = {
    stages: [
        { duration: "10s", target: 5 },
        { duration: "20s", target: 5 },
        { duration: "10s", target: 0 },
    ],

    thresholds: {
        http_req_failed: ["rate<0.01"],
        http_req_duration: ["p(95)<500"],
    },
};

const BASE_URL =
    __ENV.BASE_URL || "http://host.docker.internal:5005";

const PRODUCT_ID =
    "11111111-1111-1111-1111-111111111111";

export default function () {
    const scenarios = [
        {
            name: "Get all products",
            url: `${BASE_URL}/api/products`,
        },
        {
            name: "Get product by ID",
            url: `${BASE_URL}/api/products/${PRODUCT_ID}`,
        },
        {
            name: "Search products",
            url: `${BASE_URL}/api/products/search?q=wireless`,
        },
    ];

    const selectedScenarios =
        __ENV.SCENARIO === "search"
            ? scenarios.filter(
                (item) => item.name === "Search products"
            )
            : scenarios;

    const scenario =
        selectedScenarios[
            Math.floor(Math.random() * selectedScenarios.length)
        ];

    const response = http.get(scenario.url, {
        tags: {
            endpoint: scenario.name,
        },
    });

    check(response, {
        "status is 200": (r) => r.status === 200,
        "response time under 500ms": (r) =>
            r.timings.duration < 500,
    });

    sleep(1);
}
