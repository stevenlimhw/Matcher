# <b>Matcher</b>

## <b>Overview</b>
`Matcher` simulates how an orderbook gets updated as orders are being added and matched following a particular matching algorithm, as well as trading bots which follow various algorithmic trading strategies to take advantage of market inefficiencies. This is a side project to improve my familiarity with C#, following Object-Oriented (OO) design principles, and to expose myself further into how algorithms can make an impact in the trading scene.

## <b>Action Plan</b>
- [x] Implement TradingEngine classes
- [x] Implement LoggerModule classes
- [x] Implement classes for Order 
- [x] Implement classes for Status
- [x] Implement classes for Limit
- [x] Implement OrderCore and OrderRecord
- [x] Implement Orderbook interfaces
- [x] Implement classes for Orderbook
- [x] Implement methods for the main Book class in Orderbook
- [ ] Implement Matcher classes
- [ ] Write some mock data (sequence of orders) in csv format
- [ ] Implement DataParser classes (to parse csv and pass data to MatchingAlgorithm)
- [ ] Test the TradingEngine with the mock data
- [ ] Implement Trader classes
    - The mock data represents the orders placed by "other" traders.
    - The Trader class will be a "trader" entity that will participate in placing orders. The Trader class will follow a certain algorithm to engage in the trade.
    - Needs more research. Look up the different trading strategies and try to implement a variety to see how they fare.
- [ ] Write Developer and User Guide


## Design Decisions
### <b>Matcher</b>
<i>What is `Matcher` and how does it work?</i>
- `Matcher` is a project directory that contains the matching engines (in the folder `MatchingEngines`) and the classes for `Trade` (in the folder `Trades`).

<i>What is the class `Trade` used for?</i>
- `Trade` represents a successfully executed order. This is when a buy order and sell order get matched through the matching algorithm.
- `Trade` contains two important information: price and quantity traded.

<i>How do I implement a new matching engine?</i>
- If you would like to implement another matching engine with a different matching strategy, you can create a new class inside the folder `MatchingEngines`. 
- The class should implement the `IMatcher` interface, to ensure consistency of the functionalities the matching engines can have.