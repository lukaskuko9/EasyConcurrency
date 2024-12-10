# Pessimistic Concurrency Control
In contrary to optimistic concurrency control, pessimistic assumes the worst case scenario - that concurrencies either happen often, or they are not something that we can recover from.

## Example
Let's imagine the same scenario as with [Optimistic Concurrency](OptimisticConcurrency.md), but this time,
we postpone in our database and then also need to post these changes to some external service through Http. 
For business reason we can make the write changes only once - when postponing.

1. A request is sent to your endpoint with Id of the payment to postpone
2. You check if the payment was already postponed
3. If the payment was not postponed yet, we postpone the payment in the external service
4. Only when it's postponed in the external service, we postpone the payment in our database
5. A response is returned of a postponed due date for the payment.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/1.svg)

## Solution with pessimistic concurrency control

![Postpone Issue](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/2.svg)
