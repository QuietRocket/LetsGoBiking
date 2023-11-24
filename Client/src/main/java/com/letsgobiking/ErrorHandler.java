package com.letsgobiking;

public class ErrorHandler {

    public String getErrorMessage(Exception e) {
        // Custom logic to return a user-friendly error message based on the type of exception
        if (e instanceof NetworkException) {
            return "Network error: Unable to connect to the routing service.";
        } else if (e instanceof InputValidationException) {
            return "Input error: " + e.getMessage();
        } else {
            // Generic error message for unspecified exceptions
            return "An unexpected error occurred: " + e.getMessage();
        }
    }

    static class NetworkException extends Exception {
        public NetworkException(String message) {
            super(message);
        }
    }

    static class InputValidationException extends Exception {
        public InputValidationException(String message) {
            super(message);
        }
    }
}
