package com.example.movie_rater.config;

import com.google.auth.oauth2.GoogleCredentials;
import com.google.firebase.FirebaseApp;
import com.google.firebase.FirebaseOptions;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.io.ClassPathResource;
import org.springframework.util.StringUtils;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;

@Configuration
public class FirebaseConfig {

    private static final String DEFAULT_CREDENTIALS_CLASSPATH = "firebase-service-account.json";

    @Value("${firebase.database.url}")
    private String databaseUrl;

    @Value("${firebase.credentials.path:}")
    private String credentialsPath;

    @Bean
    public FirebaseApp initializeFirebase() throws IOException {
        GoogleCredentials credentials = GoogleCredentials.fromStream(resolveCredentialsStream());

        FirebaseOptions options = FirebaseOptions.builder()
                .setCredentials(credentials)
                .setDatabaseUrl(databaseUrl)
                .build();

        if (FirebaseApp.getApps().isEmpty()) {
            return FirebaseApp.initializeApp(options);
        }
        return FirebaseApp.getInstance();
    }

    private InputStream resolveCredentialsStream() throws IOException {
        if (StringUtils.hasText(credentialsPath)) {
            return new FileInputStream(credentialsPath);
        }

        String envCredentialsPath = System.getenv("GOOGLE_APPLICATION_CREDENTIALS");
        if (StringUtils.hasText(envCredentialsPath)) {
            return new FileInputStream(envCredentialsPath);
        }

        return new ClassPathResource(DEFAULT_CREDENTIALS_CLASSPATH).getInputStream();
    }
}
