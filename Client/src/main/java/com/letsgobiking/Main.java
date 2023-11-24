package com.letsgobiking;

public class Main {
    public static void main(String[] args) {
        ConsoleUI ui = new ConsoleUI();
        RouteProcessor routeProcessor = new RouteProcessor();
        ErrorHandler errorHandler = new ErrorHandler();

        boolean runApp = true;
        while (runApp) {
            try {
                ui.displayWelcomeMessage();
                String[] userInput = ui.getUserInput();

                if (InputValidator.isValidInput(userInput)) {
                    ui.displayProcessing();
                    Route route = routeProcessor.processRoute(userInput[0], userInput[1]);
                    ui.displayRoute(route);
                } else {
                    ui.displayError("Invalid input. Please try again.");
                }
            } catch (Exception e) {
                ui.displayError(errorHandler.getErrorMessage(e));
            }

            runApp = ui.askForNewSearch();
        }

        ui.displayGoodbye();
    }
}
