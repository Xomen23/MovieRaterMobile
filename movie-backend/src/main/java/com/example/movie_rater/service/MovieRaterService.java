package com.example.movie_rater.service;

import com.example.movie_rater.client.ImdbClient;
import com.example.movie_rater.client.dto.ImdbSearchResponse;
import com.example.movie_rater.client.dto.ImdbTitleResponse;
import com.example.movie_rater.exception.ApiException;
import com.example.movie_rater.model.Movie;
import com.example.movie_rater.model.Review;
import com.example.movie_rater.model.User;
import com.google.firebase.database.*;
import org.springframework.http.HttpStatus;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CompletableFuture;

@Service
public class MovieRaterService {

    private final ImdbClient imdbClient;
    private final PasswordEncoder passwordEncoder;

    public MovieRaterService(ImdbClient imdbClient, PasswordEncoder passwordEncoder) {
        this.imdbClient = imdbClient;
        this.passwordEncoder = passwordEncoder;
    }

    public CompletableFuture<String> registerUser(User user) {
        CompletableFuture<String> future = new CompletableFuture<>();
        try {
            DatabaseReference ref = FirebaseDatabase.getInstance().getReference("users");
            user.setPassword(passwordEncoder.encode(user.getPassword()));

            ref.orderByChild("username").equalTo(user.getUsername()).addListenerForSingleValueEvent(new ValueEventListener() {
                @Override
                public void onDataChange(DataSnapshot snapshot) {
                    if (snapshot.exists()) {
                        future.completeExceptionally(new ApiException("Username vec postoji!", HttpStatus.BAD_REQUEST));
                    } else {
                        String userId = ref.push().getKey();
                        user.setId(userId);
                        ref.child(userId).setValue(user, (error, reference) -> {
                            if (error != null) {
                                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
                            } else {
                                future.complete(userId);
                            }
                        });
                    }
                }

                @Override
                public void onCancelled(DatabaseError error) {
                    future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
                }
            });
        } catch (Exception e) {
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
                    future.completeExceptionally(new ApiException("Korisnik nije pronadjen!", HttpStatus.UNAUTHORIZED));
                    return;
                }
                for (DataSnapshot child : snapshot.getChildren()) {
                    User user = child.getValue(User.class);
                    if (user != null && passwordEncoder.matches(password, user.getPassword())) {
                        future.complete(user);
                        return;
                    }
                }
                future.completeExceptionally(new ApiException("Pogresna sifra!", HttpStatus.UNAUTHORIZED));
            }

            @Override
            public void onCancelled(DatabaseError error) {
                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
            }
        });
        return future;
    }

    public ImdbSearchResponse searchImdbMovies(String query) {
        return imdbClient.searchMovies(query, 10);
    }

    public ImdbTitleResponse getImdbMovieById(String imdbId) {
        return imdbClient.getMovieById(imdbId);
    }

    public CompletableFuture<Void> saveMovieAndReview(Review review, Movie movie) {
        CompletableFuture<Void> future = new CompletableFuture<>();
        DatabaseReference movieRef = FirebaseDatabase.getInstance().getReference("movies").child(movie.getId());
        DatabaseReference reviewRef = FirebaseDatabase.getInstance().getReference("reviews");

        movieRef.setValue(movie, (error1, ref1) -> {
            if (error1 != null) {
                future.completeExceptionally(new ApiException(error1.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
                return;
            }
            String reviewId = reviewRef.push().getKey();
            review.setId(reviewId);
            reviewRef.child(reviewId).setValue(review, (error2, ref2) -> {
                if (error2 != null) {
                    future.completeExceptionally(new ApiException(error2.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
                } else {
                    future.complete(null);
                }
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
                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
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
                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
            }
        });
        return future;
    }

    public CompletableFuture<Void> deleteReview(String reviewId) {
        CompletableFuture<Void> future = new CompletableFuture<>();
        DatabaseReference ref = FirebaseDatabase.getInstance().getReference("reviews").child(reviewId);

        ref.removeValue((error, reference) -> {
            if (error != null) {
                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
            } else {
                future.complete(null);
            }
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
                    future.completeExceptionally(new ApiException("Korisnik nije pronadjen!", HttpStatus.NOT_FOUND));
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
                future.completeExceptionally(new ApiException(error.getMessage(), HttpStatus.INTERNAL_SERVER_ERROR));
            }
        });
        return future;
    }
}
