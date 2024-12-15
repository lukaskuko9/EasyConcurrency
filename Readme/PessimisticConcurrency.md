# Pessimistic Concurrency Control
In contrary to optimistic concurrency control, pessimistic assumes the worst case scenario - that concurrencies either happen often, or they are not something that we can recover from.

## Example
This time, let's imagine a scenario where we have a database table will with data we need to process - let's say it's a payment request.

In our imaginary solution the users of our application pay us for some service. In our API requests they verify through their bank account provider. 
Once verified, we add the payment request to database table. 
The payment requests should then be processed as part of our job application.
We only provide our services once user has paid for them.

The number of rows in the table rises as the number of payments we need to process is increasing every now and then.

1. A payment request is fetched from database table
2. We check if we can process the payment
3. Payment is processed first with POST request to the bank service
4. Only when it's processed in the bank service we write changes to our database (process it on our side, give user what he paid for)
5. Message is marked as processsed

### Single instance Example
With a single request we do not have any issues.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/1.svg)

### Multiple instances Example
Since our imaginary services are very popular, we are getting many new payments each second that we need to process. 
But it takes time and with only one instance the number of payment requests is rising faster than we are able to process it.

We can't make any further significant optimalizations of the flow on our side, so we decide to process messages in parallel.

#### Without any concurrency handling
With multiple job instances in parallel, there is a chance (be it small or high) that at least two instances will run the same query at the same time,
taking the same payment. 

Multiple pass through the check if we can proceed with the payment because it was not processed yet, then all processing the same payment.
We have sucessfuly processed the payment - but our customer has paid multiple times the same amount that he was supposed to pay.

With multiple multiple instances things get messy and without any concurrency handling we process the payment multiple times in both the bank service and our database.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/2.svg)

#### Solution with pessimistic concurrency control
Using only the optimistic concurrency will not help us here because we need to make sure when calling the external service, only one instance will call it for a single payment. 
We can lock the message for a specific amount of time. This is done with the same way we detect optimistic concurrency, to make sure only one process is able to lock the message - through concurrency token. 

Once locked, we are sure that only the instance that locked the message will be able to enter the critical section to process it.
Once we are done with processing, we can mark the message as processed and unlock it.

1. A message is fetched from database table
2. An attempt to lock the message is made
3. We check if we can process the payment
4. We continue with processing the message only if we have successfully locked the message and we can process the payment
5. Payment is processed first with POST request to the bank service
6. Only when it's processed in the bank service we write changes to our database (process it on our side)
7. Message is marked as processsed

![Postpone Issue](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/3.svg)
