# coffeeshop-on-microservices

The .NET coffeeshop application 

# Services

<table>
    <thead>
        <td>No.</td>
        <td>Service Name</td>
        <td>URI</td>
    </thead>
    <tr>
        <td>1</td>
        <td>product-service</td>
        <td>http://localhost:5001 and http://localhost:15001</td>
    </tr>
    <tr>
        <td>2</td>
        <td>counter-service</td>
        <td>http://localhost:5002</td>
    </tr>
    <tr>
        <td>3</td>
        <td>barista-service</td>
        <td>http://localhost:5003</td>
    </tr>
    <tr>
        <td>4</td>
        <td>kitchen-service</td>
        <td>http://localhost:5004</td>
    </tr>
    <tr>
        <td>5</td>
        <td>reverse-proxy</td>
        <td>http://localhost:5000</td>
    </tr>
    <tr>
        <td>6</td>
        <td>signalr-web</td>
        <td>http://localhost:3000</td>
    </tr>
    <tr>
        <td>7</td>
        <td>datagen-app</td>
        <td></td>
    </tr>
</table>

# Get starting

Control plane UI:

- RabbitMQ UI: [http://localhost:15672](http://localhost:15672)
- Zipkin UI: [http://localhost:9411](http://localhost:9411)
- Jaeger UI: [http://localhost:16686](http://localhost:16686)
- Prometheus UI: [http://localhost:9090](http://localhost:9090)
- ElasticSearch UI: [http://localhost:5601](http://localhost:5601)

Using [client.http](client.http) to explore the application!
