# Optimistic Concurrency Control
Optimistic concurrency control considers the best case scenario - with minimal concurrency occurence.
The idea of optimistic concurrency is that when it occurs, you can react to it and decide what to do next - how to recover from it.

You will know if concurrency occured only when attempting to save the changes you made - not before.

## Example
Imagine a simple scenario where a user can postpone his payment by 30 days, but requirements state that he can do it only once for the payment.
You have a dedicated API endpoint. It works like this:

1. A request is sent to your endpoint with Id of the payment to postpone
2. You check if the payment was already postponed
3. If the payment was not postponed yet, you postpone it in database
4. A response is returned of a postponed due date for the payment.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency/1.svg)

## Issue
This works perfectly fine - that is, until one day,
you start getting multiple simultaneous requests for the same payment at the same time.

Because all the requests are executed at the same time with difference
only a very small fraction of second between each other,
they will fetch the payment from database and (in the worst case scenario)
in all the requests postpone has not been done yet - meaning they can bypass the condition.

The result as diagram below indicates - is that the payment was postponed 2 times, each request postponed it by 60 days.

![Postpone Issue](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency/2.svg)

## Solution
Solution lies in detecting the moment this happens.
When you fetch the payment to postpone,
you also fetch concurrency token with some value.
Your fetched concurrency token will be checked against the one stored in database
before writing the changes to database.

If the tokens match, that means no other request postponed the payment and changes are written to database.
If the tokens don't match, your payment was postponed after you fetched the payment from database,
and an exception is raised (instead of postponing again).

You can catch this exception and recover from this depending on your use case.

![Postpone Issue](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/OptimisticConcurrency/3.svg)