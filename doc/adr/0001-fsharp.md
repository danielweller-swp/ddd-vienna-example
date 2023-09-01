# 1. F# as programming language

Date: 2023-08-14

## Status

Accepted

## Context

For the implementation of GPS signal aggregation, we are looking for a programming language that

1. is well-supported on Google Cloud Platform,
2. allows domain logic to be implemented in a straightforward and reabable manner,
3. is a good fit for our team's skill set.

## Decision

From the points above, we narrowed down the decision to Kotlin (JVM) and F# (.NET Core).

We decided to go with F#, since our team is stronger and has more experience with the
.NET ecosystem as opposed to the Java ecosystem.

## Consequences

The standard programming language of the project will be F#. The runtime will be .NET Core.
