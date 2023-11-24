package com.letsgobiking;

import org.apache.cxf.jaxws.JaxWsProxyFactoryBean;
import com.letsgobiking.wsdl.*;


public class Main {
    public static void main(String[] args) {
        JaxWsProxyFactoryBean factory = new JaxWsProxyFactoryBean();
        factory.setServiceClass(IService.class);
        factory.setAddress("http://localhost:5229/Service.svc");

        IService port = (IService) factory.create();

        // Call the getData method
        String response = port.getData(10);

        // Print the response
        System.out.println(response);
    }
}