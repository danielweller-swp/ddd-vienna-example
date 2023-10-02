# DDD Vienna Example

This repository contains sample code for a talk given
by [Christian Folie](https://github.com/Nagelfar) and
[Daniel Weller](https://github.com/danielweller-swp) at
the [DDD Vienna meetup](https://www.meetup.com/ddd-vienna/) on
September 28, 2023.

## How to use this repository

The repository contains the code of a sample application
developed according to *strategic domain-driven design (DDD)*
principles.

The intended purpose of the application is in
the logistics domain, more precisely: to aggregate GPS signals
(published by trucks that perform transports of goods)
and forward them to customers, so that the customers are aware
of the positions of the trucks.

The domain, and hence our application, is called **signals**.

Since iteration and evolution are core aspects of software development
in general, and DDD in particular, the repository contains
3 versions of the signals domain, based on the passing of time: 
- an [early version](https://github.com/danielweller-swp/ddd-vienna-example/tree/early),
- a ["mid" version](https://github.com/danielweller-swp/ddd-vienna-example/tree/mid), and
- a [late version](https://github.com/danielweller-swp/ddd-vienna-example/tree/late).

These versions are modelled as branches in this git repository.

The intention is to show how how our domain code evolved over time. Each
version is usable (dev experience, deployment, ...) in a standalone way
- just check out one of the branches and explore it!

To see the changes from one version to another, you can try
[diff2html](https://diff2html.xyz/), e.g. `diff2html -- early mid` to
show the diff from the early to the mid version.