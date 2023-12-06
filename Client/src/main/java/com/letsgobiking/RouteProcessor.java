package com.letsgobiking;

import org.apache.cxf.jaxws.JaxWsProxyFactoryBean;
import com.letsgobiking.wsdl.*;

import jakarta.jms.*;

import org.datacontract.schemas._2004._07.router.*;

import java.util.ArrayList;
import java.util.List;

import org.apache.activemq.ActiveMQConnectionFactory;

public class RouteProcessor {
    private static final String ACTIVE_MQ_BROKER_URL = "tcp://localhost:61616";
    private static final String ACTIVE_MQ_USERNAME = "artemis";
    private static final String ACTIVE_MQ_PASSWORD = "artemis";

    public Route processRoute(String origin, String destination) {
        JaxWsProxyFactoryBean factory = new JaxWsProxyFactoryBean();
        factory.setServiceClass(IService.class);
        factory.setAddress("http://localhost:5229/Service.svc");

        IService routeService = (IService) factory.create();
        RouteResponseWithoutSegments serviceResponse = routeService.getBikeRouteWithQueue(origin, destination);

        // Get DirectionSegments from ActiveMQ
        String routeIdentifier = serviceResponse.getRouteIdentifier().getValue();
        List<String> directionSegments = getDirectionSegmentsFromQueue(routeIdentifier);

        Route route = new Route();

        route.setOrigin(serviceResponse.getOrigin().getValue());
        route.setDestination(serviceResponse.getDestination().getValue());
        route.setDistance(serviceResponse.getTotalDistance());
        route.setDuration(serviceResponse.getTotalDuration());

        for (String segment : directionSegments) {
            route.addDirection(segment);
        }

        return route;
    }

    private List<String> getDirectionSegmentsFromQueue(String routeIdentifier) {
        List<String> segments = new ArrayList<>();
        try {
            ConnectionFactory factory = new ActiveMQConnectionFactory(ACTIVE_MQ_USERNAME, ACTIVE_MQ_PASSWORD,
                    ACTIVE_MQ_BROKER_URL);
            Connection connection = factory.createConnection();
            connection.start();

            Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
            Queue queue = session.createQueue("segments"); // Replace with your queue name
            MessageConsumer consumer = session.createConsumer(queue, "JMSCorrelationID = '" + routeIdentifier + "'");

            TextMessage message;
            do {
                message = (TextMessage) consumer.receive();
                if (message != null) {
                    segments.add(message.getText());
                    if (message.getText().equals("Arrived!")) {
                        break;
                    }
                }
            } while (message != null);

            session.close();
            connection.close();
        } catch (JMSException e) {
            e.printStackTrace();
        }
        return segments;
    }
}
