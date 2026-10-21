# Notifly

Notifly is an automated, serverless flight price monitoring and alerting service hosted on Azure. It continuously tracks specified flight routes via third-party Global Distribution System (GDS) APIs and sends structured price alerts directly to WhatsApp when a route drops below a defined price threshold.

---

## Project Purpose & Results

This service was built and deployed to track flight deals automatically. It was successfully used to monitor and secure flight bookings from Dublin (DUB) to Phuket, Thailand (HKT) for May 2026 at an optimal target price.

---

## Technical Architecture & Tech Stack

* **Framework:** .NET 8 (Isolated Worker Process)
* **Cloud Infrastructure:** Azure Functions (Serverless / Timer Trigger)
* **External APIs:**
  * **Amadeus Flight Offers Search API:** Fetches real-time flight pricing and itinerary details.
  * **Meta WhatsApp Graph API:** Delivers notification alerts.
* **Logging & Telemetry:** `Microsoft.Extensions.Logging` integrated with Azure Application Insights.

---

## Key Features

* **Serverless Execution:** Built using Azure Functions timer triggers to execute scheduled price checks, eliminating the need to maintain a 24/7 dedicated server.
* **Meta API Compliance:** Implemented pre-approved WhatsApp Business Message Templates (`en_GB`) with positional parameters to send outbound notifications outside of Meta's 24-hour customer service window.
* **API Quota Optimization:** Utilized in-memory static caching for immutable flight and airline reference data to reduce external API calls and stay within rate limits.
* **Error Handling & Telemetry:** Integrated logging to monitor request/response lifecycles, Graph API status codes, and schema validation.

---

## System Flow

1. **Trigger:** An Azure Function timer executes on a set schedule.
2. **Fetch Pricing:** The service queries the Amadeus API for flight options matching target origin, destination, and date ranges.
3. **Threshold Evaluation:** If the lowest available fare is less than or equal to the configured price threshold, an alert payload is generated.
4. **Notification Dispatch:** The client formats the alert details into a WhatsApp template request and posts it via the Meta Graph API to the recipient's phone number.

---

## Local Development Setup

### Prerequisites
* .NET 8 SDK
* Azure Functions Core Tools v4
* Meta Developer Account (WhatsApp Business Cloud API)
* Amadeus Developer Account (API Key & Secret)
