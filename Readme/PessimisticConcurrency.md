# Pessimistic Concurrency Control
In contrary to optimistic concurrency control, pessimistic assumes the worst case scenario - that concurrencies either happen often, or they are not something that we can recover from.

## Example
This time, let's imagine a scenario where we have a database table will with data we need to process - let's say it's payment request. The number of rows in the table rises as the number of payments we need to process is increasing every now and then.

We first process the payment in our database and then also need to send a POST request an external bank service through Http for the money transfer itself.

1. A message is fetched from database table
2. Payment is processed first with POST request to the bank service
3. 4. Only when it's processed in the bank service we write changes to our database (process it on our side)
4. Message is marked as processsed

### Single instance Example
With a single request we do not have any issues.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/1.svg)

### Multiple instances Example
Since our imaginary payment solution is very popular, we are getting many new payments each second that we need to process. 
But it takes time and with only one instance the number of payment requests is rising faster than we are able to process it.

#### Without any concurrency handling
With multiple multiple instances things get messy and without any concurrency handling we process the payment multiple times in both the bank service and our database.

![Postpone Endpoint](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/2.svg)

#### Solution with pessimistic concurrency control
Using the optimistic concurrency will not help us here because we need to make sure when calling the external service, only one instance will call it for a single payment. 
Using pessimistic concurrency, we can lock it for a specific amount of time. Once locked, we are sure that only the instance that locked it will be able to enter the critical section.
Once we are done with processing, we can mark the message as processed and unlock it.

1. A message is fetched from database table
2. An attempt to lock the message is made
3. We continue with processing the message only if we have successfully locked the message
4. Payment is processed first with POST request to the bank service
5. Only when it's processed in the bank service we write changes to our database (process it on our side)
6. Message is marked as processsed

![Postpone Issue](https://github.com/lukaskuko9/EasyConcurrency/blob/readmes/Readme/PessimisticConcurrency/3.svg)
