package com.letsgobiking;

import java.util.Scanner;

public class ConsoleUI {
    private Scanner scanner;

    public ConsoleUI() {
        scanner = new Scanner(System.in);
    }

    public void displayWelcomeMessage() {
        System.out.println("Welcome to Let's Go Biking!");
        System.out.println("Enter your journey details to get started.");
    }

    public String[] getUserInput() {
        System.out.print("Enter origin: ");
        String origin = scanner.nextLine();

        System.out.print("Enter destination: ");
        String destination = scanner.nextLine();

        return new String[] {origin, destination};
    }

    public void displayProcessing() {
        System.out.println("Finding the best route...");
    }

    public void displayRoute(Route route) {
        System.out.println("Route Details:");
        System.out.println(route);
    }

    public void displayError(String errorMessage) {
        System.out.println("Error: " + errorMessage);
    }

    public boolean askForNewSearch() {
        System.out.print("Would you like to perform a new search? (yes/no): ");
        String response = scanner.nextLine();
        return response.equalsIgnoreCase("yes");
    }

    public void displayGoodbye() {
        System.out.println("Thank you for using Let's Go Biking! Goodbye.");
    }
}