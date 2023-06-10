# TradingServer

### <b>Overview</b>
This repository contains the multiple C# projects required to run the `TradingEngine`. The engine will be able to simulate how an orderbook changes as orders are being made, following a matching algorithm. It also contains trading bots, which are implemented to follow various algorithmic trading strategies.

### <b>Progress</b>
- [x] TradingEngine classes
- [x] LoggerModule classes
- [x] classes for Order 
- [x] classes for Status
- [x] classes for Limit
- [x] OrderCore and OrderRecord
- [x] Orderbook interfaces
- [x] classes for Orderbook
- [x] methods for the main Book class in Orderbook
- [ ] MatchingAlgorithm classes
- [ ] Write some mock data (sequence of orders) in csv format
- [ ] DataParser classes (to parse csv and pass data to MatchingAlgorithm)
- [ ] Test the TradingEngine with the mock data
- [ ] Create Trader classes
    - The mock data represents the orders placed by "other" traders.
    - The Trader class will be a "trader" entity that will participate in placing orders. The Trader class will follow a certain algorithm to engage in the trade.
    - Needs more research. Look up the different trading strategies and try to implement a variety to see how they fare.

