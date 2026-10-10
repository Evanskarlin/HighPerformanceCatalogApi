# Performance Benchmark Results

## 1. Overview

This document presents the results of local performance benchmarks conducted on HighPerformanceCatalogApi, an ASP.NET Core 10 Product Catalog and Search API integrating PostgreSQL, Redis, and Elasticsearch.

The objective was to establish a reproducible performance baseline and assess API response latency, throughput, and reliability under a small concurrent workload.

Two benchmark scenarios were executed using k6: a mixed API workload and a search-only workload.

## 2. Test Environment

| Component | Configuration |
|---|---|
| Operating system | Ubuntu 24.04 LTS |
| CPU | Intel Core i7-12650H, 16 logical CPUs reported |
| System memory | 15 GiB |
| .NET SDK | 10.0.112 |
| Database | PostgreSQL 18 |
| Cache | Redis 8 |
| Search engine | Elasticsearch 9.5.4 |
| Docker | 29.8.0 |
| Load testing tool | k6 2.3.0 |
| API endpoint | http://localhost:5005 |
| Test date | 10 October 2026 (NZDT) |

The API, supporting infrastructure, and k6 load generator were executed on the same development machine using Docker.

These measurements represent local development performance rather than production capacity.

## 3. Test Methodology

The benchmarks were conducted using k6 with the following configuration:

- Ramp-up: 10 seconds, increasing to five virtual users.
- Steady state: 20 seconds with five virtual users.
- Ramp-down: 10 seconds, decreasing to zero virtual users.
- One-second pause between iterations.
- HTTP status code expected: 200.
- Individual response-time check: below 500 ms.

The configured performance thresholds were:

- HTTP request failure rate below 1%.
- P95 response latency below 500 ms.

### Scenario A: Mixed API Workload

The mixed workload randomly selected one of three endpoints:

1. `GET /api/products`
2. `GET /api/products/{id}`
3. `GET /api/products/search?q=wireless`

This scenario evaluates API responsiveness across product listing, individual product retrieval, and product search.

### Scenario B: Search-Only Workload

The search-only workload repeatedly executed:

`GET /api/products/search?q=wireless`

The search cache was cleared before the benchmark, allowing the first uncached request to populate Redis. Subsequent requests could retrieve cached results.

This represents a predominantly warm-cache workload rather than an isolated cold-cache benchmark.

## 4. Benchmark Results

| Metric | Mixed workload | Search-only workload |
|---|---:|---:|
| Total HTTP requests | 156 | 156 |
| Throughput (requests/sec) | 3.88 | 3.89 |
| Average latency | 2.78 ms | 2.54 ms |
| Median latency | 2.62 ms | 2.63 ms |
| P95 latency | 4.39 ms | 3.75 ms |
| Maximum latency | 13.69 ms | 9.41 ms |
| HTTP failure rate | 0.00% | 0.00% |
| Successful checks | 312/312 | 312/312 |

Both workloads satisfied the configured k6 performance thresholds.

## 5. Performance Analysis

### 5.1 Response Latency

The mixed workload achieved an average response latency of 2.78 ms, while the search-only workload achieved 2.54 ms.

The search-only workload recorded approximately 8.5% lower average latency.

However, this difference cannot be attributed exclusively to Redis caching because the two workloads contain different endpoint combinations.

### 5.2 P95 Latency

The P95 response latency was 4.39 ms for the mixed workload and 3.75 ms for the search-only workload.

This indicates that approximately 95% of measured HTTP requests completed within these respective durations.

Both results were substantially below the configured 500 ms threshold.

### 5.3 Throughput

The mixed workload processed approximately 3.88 requests per second, while the search-only workload processed approximately 3.89 requests per second.

The similar throughput reflects the test's low concurrency and one-second pause between iterations.

These results should not be interpreted as the maximum throughput capacity of the API.

### 5.4 Reliability

Both benchmark scenarios completed 156 HTTP requests without recorded HTTP failures.

Each scenario also passed all 312 checks, covering successful HTTP responses and individual response times below 500 ms.

This indicates reliable request handling under the tested conditions.

### 5.5 Redis Cache Behaviour

The search-only scenario repeatedly executed the same query, allowing Redis to serve subsequent requests after initial cache population.

The measured average latency of 2.54 ms and P95 latency of 3.75 ms demonstrate low response times for this workload.

However, these measurements do not establish a quantitative Redis speedup over Elasticsearch because a controlled cache HIT versus MISS comparison was not performed.

## 6. Limitations

The benchmark has several limitations:

1. The API and load generator shared the same physical machine.
2. Only five concurrent virtual users were tested.
3. The total benchmark duration was approximately 40 seconds.
4. The one-second sleep interval limited the generated request rate.
5. The mixed workload used a limited product dataset and fixed search query.
6. The search-only scenario was predominantly warm-cache.
7. Cache HIT and MISS latencies were not measured independently.
8. CPU, memory, and container resource utilisation were not recorded during the test.
9. Network conditions do not represent a distributed production environment.
10. Each workload was measured once, without repeated trials to quantify variability.

Therefore, the results should be interpreted as a local performance baseline, not a production scalability guarantee.

## 7. Future Improvements

Further benchmarking could include:

- Controlled Redis HIT versus MISS comparisons.
- Separate PostgreSQL and Elasticsearch read-path measurements.
- Increased concurrency levels of 10, 25, 50, and 100 virtual users.
- Longer-duration load tests.
- Per-endpoint latency and throughput reporting.
- CPU and memory utilisation monitoring.
- Repeated trials with statistical comparison.
- Testing infrastructure failure and PostgreSQL fallback performance.

## 8. Reproducibility

The benchmark script is located at:

`tests/load/catalog-load-test.js`

The metric extraction utility is located at:

`tests/load/summarize_results.py`

Generated benchmark summaries are stored locally in:

`tests/load/results/`

The benchmark can be reproduced using Docker and k6, with the API accessible at `http://localhost:5005`.

## 9. Conclusion

The HighPerformanceCatalogApi demonstrated low response latency and no HTTP failures under the tested five-virtual-user workload.

The mixed workload achieved an average latency of 2.78 ms and P95 latency of 4.39 ms. The search-only workload achieved an average latency of 2.54 ms and P95 latency of 3.75 ms.

These results provide an initial performance baseline for the project and support further investigation into caching effectiveness, database performance, and scalability under higher concurrency.