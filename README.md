RabbitMQ.LoadTest
=================

Load testing application for RabbitMQ with EasyNetQ

RabbitMQ.LoadTest comes in two parts, a publisher and a subscriber. In order to test the throughput of a given rabbitmq 
server start the subscribers and then start the publishers. It is recommended to run multiple copies of each attached to 
separate vhosts to increase the load. This is of course dependant on the environment you will be using RabbitMQ in. No 
need for many queues and connections if your usage will be light.

See Wiki for usage information and examples
