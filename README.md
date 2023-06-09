## Trading Server using C#

## Orderbook
### Interfaces
1. `IReadOnlyOrderbook` - read only (weakest permission)
2. `IOrderEntryOrderbook` - read and write, implements `IReadOnlyOrderbook`
3. `IRetrievalOrderbook` - allows mutation of orderbook outside of the Orderbook class (strongest permission), implements `IOrderEntryOrderbook`
4. `IMatchingOrderbook` - implements `IRetrievalOrderbook` (strongest permission)

### Implementations
1. `Orderbook` implements `IRetrievalOrderbook`.
2. 