package com.letsgobiking;

import org.apache.cxf.jaxws.JaxWsProxyFactoryBean;
import com.letsgobiking.wsdl.*;
import org.datacontract.schemas._2004._07.router.*;

public class RouteProcessor {

    public Route processRoute(String origin, String destination) {
        JaxWsProxyFactoryBean factory = new JaxWsProxyFactoryBean();
        factory.setServiceClass(IService.class);
        factory.setAddress("http://localhost:5229/Service.svc");

        IService routeService = (IService) factory.create();
        RouteResponse serviceResponse = routeService.getBikeRoute(origin, destination);

        return parseServiceResponse(serviceResponse);
    }

    private Route parseServiceResponse(RouteResponse response) {
        Route route = new Route();
        route.setOrigin(response.getOrigin().getValue());
        route.setDestination(response.getDestination().getValue());
        route.setDistance(response.getTotalDistance());
        route.setDuration(response.getTotalDuration());

        for (DirectionSegment segment : response.getDirectionSegments().getValue().getDirectionSegment()) {
            route.addDirection(segment.getInstruction().getValue(), segment.getDistance().doubleValue(),
                    segment.getDuration().longValue());
        }

        return route;
    }
}
