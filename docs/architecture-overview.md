# Architecture Overview
The document describes the overall architecture of Apocryph Discord Bot, 
including its modules, dependencies, technology choices and patterns.
It is highly recommended read for anyone interested in contributing to the
Apocryph Discord Bot.

## Diagram

## Modules
Apocryphh Discord Bot is broken down into three main modules:
1. Core - .NET 5.0 application based on Perper responsible for bot logic
2. Frontend - HTML / JS micro-fronteds used for presenting more complex data / interactions
3. Ethereum Node - Etheremum Parity Node used to send transactions and execute queries to Ethereum network

### Core
The core modules is implemented as a singe Perper Agent with the following streams:
[TODO: Add description of all streams / calls]

### Fronted
[TODO: Add description of the different user interfaces]

### Ethereum Node
[TODO: Add description of the parity node]

## Operations
All of the core modules are packaged as containers and are orchestrated using
Kubernetes. 