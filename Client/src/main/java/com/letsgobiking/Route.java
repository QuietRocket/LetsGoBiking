package com.letsgobiking;

import java.util.ArrayList;
import java.util.List;

public class Route {
    private String origin;
    private String destination;
    private double distance; // in kilometers
    private long duration; // in minutes
    private List<DetailedDirection> detailedDirections;

    public Route() {
        detailedDirections = new ArrayList<>();
    }

    public void addDirection(String instruction, double segmentDistance, long segmentDuration) {
        DetailedDirection direction = new DetailedDirection(instruction, segmentDistance, segmentDuration);
        detailedDirections.add(direction);
    }

    public void setOrigin(String origin) {
        this.origin = origin;
    }

    public void setDestination(String destination) {
        this.destination = destination;
    }

    public void setDistance(double distance) {
        this.distance = distance;
    }

    public void setDuration(long duration) {
        this.duration = duration;
    }

    @Override
    public String toString() {
        StringBuilder sb = new StringBuilder();
        sb.append("Route from ").append(origin).append(" to ").append(destination).append(":\n");
        sb.append("Total Distance: ").append(distance).append(" km\n");
        sb.append("Estimated Total Duration: ").append(duration).append(" minutes\n");
        sb.append("Detailed Directions:\n");
        for (DetailedDirection direction : detailedDirections) {
            sb.append(direction).append("\n");
        }
        return sb.toString();
    }

    private class DetailedDirection {
        private String instruction;
        private double distance; // Distance for this segment
        private long duration; // Duration for this segment

        public DetailedDirection(String instruction, double distance, long duration) {
            this.instruction = instruction;
            this.distance = distance;
            this.duration = duration;
        }

        @Override
        public String toString() {
            return instruction + " (Distance: " + distance + " km, Duration: " + duration + " minutes)";
        }
    }
}
