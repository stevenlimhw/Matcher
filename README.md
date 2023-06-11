# <b>TradingServer</b>

## <b>Overview</b>
TradingServer contains a trading engine that simulates how an orderbook changes as orders are being added and matched (following a matching algorithm), as well as trading bots which follow various algorithmic trading strategies.

This is a side project to improve my familiarity with the .NET language C#, following Object-Oriented (OO) design principles, and to expose myself further into how algorithms can make an impact in the trading scene.

The project is based on the [YouTube series by Coding Jesus](https://youtube.com/playlist?list=PLIkrF4j3_p-3fA9LyzSpT6yFPnqvJ02LS), but with some modifications to the implementation. Feel free to refer to this repository for inspiration when attempting your own trading engine project, and feel free to leave me some feedback as well! 

However, due to the fact that the series did not really provide the full implementation of the order book and did not touch on the implementation of the matching algorithm at all, the code written for these parts are fully written by me (who isn't a professional in this field, so err on the side of caution when referencing my code).


## <b>Action Plan</b>
- [x] TradingEngine classes
- [x] LoggerModule classes
- [x] classes for Order 
- [x] classes for Status
- [x] classes for Limit
- [x] OrderCore and OrderRecord
- [x] Orderbook interfaces
- [x] classes for Orderbook
- [x] methods for the main Book class in Orderbook
- [ ] Matcher classes
- [ ] Write some mock data (sequence of orders) in csv format
- [ ] DataParser classes (to parse csv and pass data to MatchingAlgorithm)
- [ ] Test the TradingEngine with the mock data
- [ ] Create Trader classes
    - The mock data represents the orders placed by "other" traders.
    - The Trader class will be a "trader" entity that will participate in placing orders. The Trader class will follow a certain algorithm to engage in the trade.
    - Needs more research. Look up the different trading strategies and try to implement a variety to see how they fare.
- [ ] Developer and User Guide


## <b>Matcher Implementation</b>
<i>What is `Matcher` and how does it work?</i>
- `Matcher` is a project directory that contains the matching engines (in the folder `MatchingEngines`) and the classes for `Trade` (in the folder `Trades`).

<i>What is the class `Trade` used for?</i>
- `Trade` represents a successfully executed order. This is when a buy order and sell order get matched through the matching algorithm.
- `Trade` contains two important information: price and quantity traded.

<i>How do I implement a new matching engine?</i>
- If you would like to implement another matching engine with a different matching strategy, you can create a new class inside the folder `MatchingEngines`. 
- The class should implement the `IMatcher` interface, to ensure consistency of the functionalities the matching engines can have.