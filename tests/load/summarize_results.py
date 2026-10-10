#!/usr/bin/env python3

import json
import sys
from pathlib import Path


def summarize(path):
    with path.open(encoding="utf-8") as file:
        data = json.load(file)

    metrics = data.get("metrics", {})

    def value(name, key):
        metric = metrics.get(name, {})

        # Supports both direct and nested k6 formats.
        values = metric.get("values", metric)

        return values.get(key)

    requests = value("http_reqs", "count")
    throughput = value("http_reqs", "rate")

    average = value("http_req_duration", "avg")
    median = value("http_req_duration", "med")
    p95 = value("http_req_duration", "p(95)")
    maximum = value("http_req_duration", "max")

    failure_rate = value("http_req_failed", "value")
    passed = value("checks", "passes")
    failed = value("checks", "fails")

    def display(number, decimals=2):
        return (
            f"{number:.{decimals}f}"
            if number is not None
            else "N/A"
        )

    print(f"\nBenchmark: {path.name}")
    print("-" * 45)
    print(f"Total requests: {requests}")
    print(f"Requests/second: {display(throughput)}")
    print(f"Average latency: {display(average)} ms")
    print(f"Median latency: {display(median)} ms")
    print(f"P95 latency: {display(p95)} ms")
    print(f"Maximum latency: {display(maximum)} ms")
    print(
        "HTTP failure rate:",
        f"{failure_rate * 100:.2f}%"
        if failure_rate is not None
        else "N/A"
    )
    print(f"Checks passed: {passed}")
    print(f"Checks failed: {failed}")


if __name__ == "__main__":
    if len(sys.argv) < 2:
        print(
            "Usage: python3 summarize_results.py "
            "<result.json> [result.json ...]"
        )
        sys.exit(1)

    for filename in sys.argv[1:]:
        path = Path(filename)

        if not path.is_file():
            print(f"File not found: {path}")
            sys.exit(1)

        summarize(path)