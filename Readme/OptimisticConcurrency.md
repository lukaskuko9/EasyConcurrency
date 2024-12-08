## Optimistic Concurrency Control
Optimistic concurrency control considers the best case scenario - with minimal concurrency occurence.

### Example

Imagine a simple scenario where a user can postpone his payment by 30 days, but he can do it only once for the payment.
You have a dedicated endpoint. It works like this:

1. A request is sent to your endpoint with Id of the payment to postpone
2. Payment is fetched from database
3. You check if the payment was already postponed
4. If the payment was not postponed yet, you postpone it
5. A response is returned of a postponed due date for the payment.

It works perfectly fine - that is, until one day,
you start getting 10 requests like this for the same payment at the same time.

Because all the requests are executed at the same time with difference
only a very small fraction of second between each other,
they will fetch the payment from database and (in the worst case scenario)
in all the requests postpone has not been done yet - meaning they can bypass the 4th step, the condition.

The result is that the payment was postponed 10 times, each request postponed it by 30 days.

