package com.letsgobiking;

public class InputValidator {

    public static boolean isValidInput(String[] inputs) {
        // Check if inputs are not null and have exactly 2 elements (origin and destination)
        if (inputs == null || inputs.length != 2) {
            return false;
        }

        // Validate each input
        return isValidLocation(inputs[0]) && isValidLocation(inputs[1]);
    }

    private static boolean isValidLocation(String input) {
        if (input == null || input.trim().isEmpty()) {
            return false;
        }

        int length = input.length();
        if (length < 3 || length > 100) {
            return false;
        }

        for (char c : input.toCharArray()) {
            if (!Character.isLetterOrDigit(c) && " .,-'".indexOf(c) == -1) {
                return false;
            }
        }

        return true;
    }
}
