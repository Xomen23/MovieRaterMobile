package com.example.movie_rater.service;

import com.example.movie_rater.client.ImdbClient;
import com.example.movie_rater.client.dto.ImdbSearchResponse;
import com.example.movie_rater.model.Movie;
import com.example.movie_rater.model.Review;
import com.example.movie_rater.model.User;
import com.google.firebase.database.*;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

@Service
public class MovieRaterService {

    private final ImdbClient imdbClient;

    public MovieRaterService(ImdbClient imdbClient) {
        this.imdbClient = imdbClient;
    }

    /*
    public CompletableFuture<String> registerUser(User user) {
        CompletableFuture<String> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("users");

        ref.orderByChild("username").equalTo(user.getUsername()).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot snapshot) {
                if (snapshot.exists()) {
                    future.completeExceptionally(new RuntimeException("Username vec postoji!"));
                } else {
                    String userId = ref.push().getKey();
                    user.setId(userId);
                    ref.child(userId).setValue(user, (error, reference) -> {
                        if (error != null) future.completeExceptionally(error.toException());
                        else future.complete(userId);
                    });
                }
            }
            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(error.toException());
            }
        });
        return future;
    }*/
    public CompletableFuture<String> registerUser(User user) {
        CompletableFuture<String> future = new CompletableFuture<>();
        try {
            DatabaseReference ref = FirebaseDatabase.getInstance().getReference("users");

            ref.orderByChild("username").equalTo(user.getUsername()).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot snapshot) {
                    if (snapshot.exists()) {
                        future.completeExceptionally(new RuntimeException("Username vec postoji!"));
                    } else {
                        String userId = ref.push().getKey();
                        user.setId(userId);
                        ref.child(userId).setValue(user, (error, reference) -> {
                            if (error != null) {
                                System.err.println("Firebase error prilikom upisa: " + error.getMessage());
                                future.completeExceptionally(error.toException());
                            } else {
                                future.complete(userId);
                            }
                        });
                    }
                }
                @Override
                public void onCancelled(DatabaseError error) {
                    System.err.println("Firebase DatabaseError: " + error.getMessage());
                    future.completeExceptionally(error.toException());
                }
            });
        } catch (Exception e) {
            System.err.println("Generalna greska u servisu: " + e.getMessage());
            e.printStackTrace();
            future.completeExceptionally(e);
        }
        return future;
    }

    public CompletableFuture<User> loginUser(String username, String password) {
        CompletableFuture<User> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("users");

        ref.orderByChild("username").equalTo(username).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot snapshot) {
                if (!snapshot.exists()) {
                    future.completeExceptionally(new RuntimeException("Korisnik nije pronadjen!"));
                    return;
                }
                for (DataSnapshot child : snapshot.getChildren()) {
                    User user = child.getValue(User.class);
                    if (user != null && user.getPassword().equals(password)) {
                        future.complete(user);
                        return;
                    }
                }
                future.completeExceptionally(new RuntimeException("Pogresna sifra!"));
            }

            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(error.toException());
            }
        });
        return future;
    }

    public ImdbSearchResponse searchImdbMovies(String query) {
        return imdbClient.searchMovies(query, 10);
    }

    public CompletableFuture<Void> saveMovieAndReview(Review review, Movie movie) {
        CompletableFuture<Void> future = new CompletableFuture<>();
        DatabaseReference movieRef = FirebaseDatabase.getInstance().getReference("movies").child(movie.getId());
        DatabaseReference reviewRef = FirebaseDatabase.getInstance().getReference("reviews");

        movieRef.setValue(movie, (error1, ref1) -> {
            if (error1 != null) {
                future.completeExceptionally(error1.toException());
                return;
            }
            String reviewId = reviewRef.push().getKey();
            review.setId(reviewId);
            reviewRef.child(reviewId).setValue(review, (error2, ref2) -> {
                if (error2 != null) future.completeExceptionally(error2.toException());
                else future.complete(null);
            });
        });
        return future;
    }

    public CompletableFuture<List<Review>> getMovieReviews(String imdbId) {
        CompletableFuture<List<Review>> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("reviews");

        ref.orderByChild("imdbId").equalTo(imdbId).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot snapshot) {
                List<Review> reviews = new ArrayList<>();
                for (DataSnapshot child : snapshot.getChildren()) {
                    Review review = child.getValue(Review.class);
                    if (review != null) {
                        reviews.add(review);
                    }
                }
                future.complete(reviews);
            }

            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(error.toException());
            }
        });
        return future;
    }

    public CompletableFuture<List<Review>> getReviewsForUser(String userId) {
        CompletableFuture<List<Review>> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("reviews");

        ref.orderByChild("userId").equalTo(userId).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot snapshot) {
                List<Review> reviews = new ArrayList<>();
                for (DataSnapshot child : snapshot.getChildren()) {
                    Review review = child.getValue(Review.class);
                    if (review != null) {
                        reviews.add(review);
                    }
                }
                future.complete(reviews);
            }

            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(error.toException());
            }
        });
        return future;
    }

    public CompletableFuture<Void> deleteReview(String reviewId) {
        CompletableFuture<Void> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("reviews").child(reviewId);

        ref.removeValue((error, reference) -> {
            if (error != null) future.completeExceptionally(error.toException());
            else future.complete(null);
        });
        return future;
    }

    public CompletableFuture<User> getUserByUsername(String username) {
        CompletableFuture<User> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("users");

        ref.orderByChild("username").equalTo(username).addListenerForSingleValueEvent(new ValueEventListener() {
            @Override
            public void onDataChange(DataSnapshot snapshot) {
                if (!snapshot.exists()) {
                    future.completeExceptionally(new RuntimeException("Korisnik nije pronadjen!"));
                    return;
                }
                for (DataSnapshot child : snapshot.getChildren()) {
                    User user = child.getValue(User.class);
                    future.complete(user);
                    return;
                }
            }

            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(error.toException());
            }
        });
        return future;
    }
}