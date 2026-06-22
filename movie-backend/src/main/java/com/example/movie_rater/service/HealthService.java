package com.example.movie_rater.service;

import com.example.movie_rater.client.ImdbClient;
import com.google.firebase.database.DataSnapshot;
import com.google.firebase.database.DatabaseError;
import com.google.firebase.database.FirebaseDatabase;
import com.google.firebase.database.ValueEventListener;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import java.util.LinkedHashMap;
import java.util.Map;
import java.util.concurrent.CompletableFuture;
import java.util.concurrent.TimeUnit;
import java.util.concurrent.TimeoutException;

@Service
public class HealthService {

    private final ImdbClient imdbClient;

    @Value("${imdb.api.url}")
    private String imdbApiUrl;

    @Value("${firebase.database.url}")
    private String firebaseDatabaseUrl;

    public HealthService(ImdbClient imdbClient) {
        this.imdbClient = imdbClient;
    }

    public Map<String, Object> getHealthStatus() {
        Map<String, Object> status = new LinkedHashMap<>();
        status.put("backend", "UP");
        status.put("firebase", checkFirebase());
        status.put("imdbApi", checkImdbApi());
        return status;
    }

    private Map<String, Object> checkFirebase() {
        Map<String, Object> firebase = new LinkedHashMap<>();
        firebase.put("url", firebaseDatabaseUrl);

        try {
            CompletableFuture<Void> future = new CompletableFuture<>();

            FirebaseDatabase.getInstance().getReference("users")
                    .limitToFirst(1)
                    .addListenerForSingleValueEvent(new ValueEventListener() {
                        @Override
                        public void onDataChange(DataSnapshot snapshot) {
                            future.complete(null);
                        }

                        @Override
                        public void onCancelled(DatabaseError error) {
                            future.completeExceptionally(new RuntimeException(error.getMessage()));
                        }
                    });

            future.get(5, TimeUnit.SECONDS);
            firebase.put("status", "UP");
            firebase.put("message", "Povezan na Firebase Realtime Database");
        } catch (TimeoutException e) {
            firebase.put("status", "DOWN");
            firebase.put("message", "Firebase nije odgovorio u roku od 5 sekundi");
        } catch (Exception e) {
            firebase.put("status", "DOWN");
            firebase.put("message", e.getMessage());
        }

        return firebase;
    }

    private Map<String, Object> checkImdbApi() {
        Map<String, Object> imdb = new LinkedHashMap<>();
        imdb.put("url", imdbApiUrl);

        try {
            imdbClient.searchMovies("inception", 1);
            imdb.put("status", "UP");
            imdb.put("message", "IMDB API dostupan");
        } catch (Exception e) {
            imdb.put("status", "DOWN");
            imdb.put("message", e.getMessage());
        }

        return imdb;
    }
}
