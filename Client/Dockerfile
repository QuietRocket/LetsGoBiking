FROM arm64v8/maven:3-eclipse-temurin-17
WORKDIR /app
COPY pom.xml .
RUN mvn dependency:go-offline
COPY . .
RUN mvn clean package
CMD ["java", "-jar", "target/heavyclient-1.0-SNAPSHOT.jar"]