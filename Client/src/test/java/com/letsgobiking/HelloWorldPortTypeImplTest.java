package com.letsgobiking;

import org.junit.Test;
import static org.junit.Assert.assertEquals;

public class HelloWorldPortTypeImplTest {

    @Test
	public void testSayHi() {
        HelloWorldPortTypeImpl helloWorldPortTypeImpl = new HelloWorldPortTypeImpl();
        String response = helloWorldPortTypeImpl.sayHi("Sam");
        assertEquals("HelloWorldPortTypeImpl not properly saying hi", "Hello Sam", response);
    }
}
