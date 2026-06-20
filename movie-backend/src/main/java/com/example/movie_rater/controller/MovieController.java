package com.example.movie_rater.controller;

import com.example.movie_rater.client.dto.ImdbSearchResponse;
import com.example.movie_rater.dto.ReviewRequest;
import com.example.movie_rater.model.Review;
import com.example.movie_rater.service.MovieRaterService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@Tag(name = "User Movie Controller", description = "Controller for searching movies and managing reviews")
@RestController
@RequestMapping("/api/movies")
@CrossOrigin(origins = "*")
public class MovieController {

    private final MovieRaterService movieRaterService;

    public MovieController(MovieRaterService movieRaterService) {
        this.movieRaterService = movieRaterService;
    }

    @Operation(summary = "Search for a movie on imdb", description = "Search on IMDB for titles using search query")
    @GetMapping("/search")
    public ResponseEntity<ImdbSearchResponse> searchMovies(@Parameter(description = "Search query") @RequestParam(value = "query") String query) {
        ImdbSearchResponse response = movieRaterService.searchImdbMovies(query);
        return ResponseEntity.ok(response);
    }

    @Operation(summary = "Add review", description = "Save a movie and add a review with rating (1-10) and comment")
    @PostMapping("/review")
    public ResponseEntity<String> addReview(@RequestBody ReviewRequest request) {
        try {
            movieRaterService.saveMovieAndReview(request.getReview(), request.getMovie()).join();
            return ResponseEntity.ok("Recenzija je uspesno sacuvana!");
        } catch (Exception e) {
            return ResponseEntity.status(500).body(e.getMessage());
        }
    }

    @Operation(summary = "Get reviews for a movie", description = "Put imdbId to see all reviews and ratings for that movie")
    @GetMapping("/{imdbId}/reviews")
    public ResponseEntity<List<Review>> getReviews(@Parameter(description = "ID from imdb") @PathVariable(value = "imdbId") String imdbId) {
        try {
            List<Review> reviews = movieRaterService.getMovieReviews(imdbId).join();
            return ResponseEntity.ok(reviews);
        } catch (Exception e) {
            return ResponseEntity.status(500).build();
        }
    }

    @Operation(summary = "Get all reviews for a user", description = "Put userId to see all movies/reviews added by that user (collection)")
    @GetMapping
    public ResponseEntity<List<Review>> getUserReviews(@Parameter(description = "ID of the user") @RequestParam(value = "userId") String userId) {
        try {
            List<Review> reviews = movieRaterService.getReviewsForUser(userId).join();
            return ResponseEntity.ok(reviews);
        } catch (Exception e) {
            return ResponseEntity.status(500).build();
        }
    }

    @Operation(summary = "Delete review", description = "Put reviewId to delete a review from the user's collection")
    @DeleteMapping("/review/{reviewId}")
    public ResponseEntity<String> deleteReview(@Parameter(description = "ID of the review") @PathVariable(value = "reviewId") String reviewId) {
        try {
            movieRaterService.deleteReview(reviewId).join();
            return ResponseEntity.ok("Recenzija je uspesno obrisana!");
        } catch (Exception e) {
            return ResponseEntity.status(500).body(e.getMessage());
        }
    }
}
