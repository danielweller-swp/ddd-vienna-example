# 9. Developing a Testing Strategy

Date: 2022-09-26

## Status

Accepted

## Context

Testing is vital for being able to continuously develop and deliver features and generate value for the customer.
A testing strategy will help to choose the right kind of test and to write the right amount of tests during  development.

In general the following testing types can be distinguished:

1. Unit tests which are tied to a specific implementation and verify the correctness of a small unit,
2. Behavior-Driven-Development (BDD) styled tests which focus on the observable behavior of a (larger) part of the system,
3. Contract tests to verify that agreements with external parties are met,
4. UI / system integration tests which verify a flow or a process within the application over multiple components.

For all types of tests the following points need to be reviewed

* choosing a technology for writing the tests,
* deciding on a test implementation strategy, e.g. is mocking of parts of the system encouraged,
* defining the desired amount of tests and the timepoint when tests should be written,
* the environment or stages where the tests are executed, e.g. developer machine, build server or test environment.

## Decision

We acknowledge that the discussed types of tests require different approaches and technologies to solve them.

### Unit tests

The goal of unit tests is to verify a very concrete behavior of a small part of the system.
In most cases this correlates to one or a couple of pure functions.

#### Technology and environment

We use [FSUnit](http://fsprojects.github.io/FsUnit/) for writing test assertions and use [XUnit](https://xunit.net/) as primary test framework.

Note: An more in depth evaluation of [Expecto](https://github.com/haf/expecto) is still missing.

Unit tests are executed on all stages and environments, but most frequently on a developer machine.

#### Implementation Strategy

We use mocking only very sparsely for things which are not under our direct control.
Unit tests must run purely in memory and should not use any technical infrastructure.

We try to group all tests for a module within one file.

#### Amount and timepoint of tests

Enough tests, so that the main branches of a unit are covered. We strive not for 80-100% test coverage.

We try to write as much unit tests upfront as possible.

### BDD Tests

These tests are not directly tied to a specific implementation, but they interact with the application in a very coarse way by calling APIs and observing side effects.

#### Technology and environment

We write tests with a Given-When-Then schema as defined in the application.

We use [FSUnit](http://fsprojects.github.io/FsUnit/) for writing test assertions and use [XUnit](https://xunit.net/) as primary test framework.

Tests are executed on all stages and environments.

#### Implementation Strategy

Similar to unit tests the goal is to execute the tests in memory and we use mocking for all communication outside of tested component.
Sometimes this pattern is called ‘Guiding Test’ or [Subcutaneous Test](https://www.martinfowler.com/bliki/SubcutaneousTest.html)
Meaning that:

* Http calls via HttpClient are intercepted, and they are recorded and stub results are returned,
* Sent or published messages are recorded by an in-memory bus and are not forwarded to other parts of the system.

Tests are usually grouped in scenarios, which are structured by the setup (given) and the when part.

#### Amount and timepoint of tests

Enough tests to cover the main scenarios of an API and to verify that connections to other system parts are called.

Some tests are written up front as a Guiding tests to verify when the desired behavior is reached.

### UI / System integration tests

These tend to touch multiple parts of the system and are brittle and not as stable in the long run.

#### Technology and environments

Currently no explicit technology has been chosen.
We can use:

* manual testing after deploying the application,
* monitor logs to verify the correctness of the application

Usually these test run slowly and therefore are executed on the build server against a test environment.

#### Implementation Strategy

The system integration tests should use technical infrastructure as similar as possible to the production environment.
This means that

* mocking or stubbing parts of the system infrastructure is not possible,
* mocking of external dependencies might be required,
* verifying the application in production is encouraged.

#### Amount and timepoint of tests

A few tests covering the main use cases of the application are enough.

These tests are usually written or defined once the API is stable enough.

### Contract tests

Contract tests have to goals:

* verify that a consumed contract behaves as expected
* verify that contracts implemented by the system to third parties behave as agreed.

#### Technology and environments

Currently no technology was chosen, probably [Pact.IO](https://pact.io/) could be a viable choice.

Contract tests should be executed on every build to continuously verify if provided or consumed contracts are still valid.

#### Implementation Strategy

TBD

#### Amount and timepoint of tests

Per consumed contract and use case a test should be written to verify the expected behavior.

For provided contracts test should be written and distributed to the consumers.

## Consequences

* We have different styles of tests in place
* We need to provide enough [testing seams](http://www.informit.com/articles/article.aspx?p=359417&seqNum=3) to allow mocking or replacing parts of the infrastructure
* We might need to include test specific code in the production code
