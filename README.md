RabbitMQ.LoadTest
=================

Load testing application for RabbitMQ with EasyNetQ

RabbitMQ.LoadTest comes in two parts, a publisher and a subscriber. In order to test the throughput of a given rabbitmq 
server start the subscribers and then start the publishers. It is recommended to run multiple copies of each attached to 
separate vhosts to increase the load. This is of course dependant on the environment you will be using RabbitMQ in. No 
need for many queues and connections if you usage will be light.

Subscribers Configration.

The RabbitMQ.LoadTest.Subscriber.Config file contains the default settings and can be adjusted as necessary. 

<appSettings>
    <add key="Host" value="localhost"/>
    <add key="Port" value="5672"/>
    <add key="VHost" value="/"/>
    <add key="RabbitMQUser" value="guest"/>
    <add key="RabbitMQPassword" value="guest"/>
    <add key="Threads" value="20"/>
    <add key="Queues" value ="10"/>    <!--minimum 1, maximum 10 should match publisher-->
  </appSettings>
  
The defaults can be overridden on the command line with the following options. (RabbitMQ.LoadTest.Subscriber.exe -h)
RabbitMQ.LoadTest 1.0.0.0
Copyright ©  2013

  -h, --host-port                 Rabbit Host to connect to ( and :port
                                  optionally)

  -v, --vhost                     Virtual host on Rabbit server to connect to

  -u, --username                  RabbitMQ Username

  -p, --password                  RabbitMQ password

  -t, --threads                   Number of threads (ideally divisble by number
                                  of queues)

  -q, --queues                    Number of queues (ideally threads a multiple
                                  of queues)

  --help                          Display this help screen.


