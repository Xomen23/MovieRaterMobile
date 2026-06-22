package com.example.movie_rater.controller;

import com.example.movie_rater.client.dto.ImdbSearchResponse;
import com.example.movie_rater.client.dto.ImdbTitleResponse;
import com.example.movie_rater.dto.ErrorResponse;
import com.example.movie_rater.dto.ReviewRequest;
import com.example.movie_rater.model.Review;
import com.example.movie_rater.service.MovieRaterService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
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
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Rezultati pretrage"),
            @ApiResponse(responseCode = "500", description = "Greska pri pretrazi",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @GetMapping("/search")
    public ResponseEntity<ImdbSearchResponse> searchMovies(
            @Parameter(description = "Search query") @RequestParam(value = "query") String query) {
        ImdbSearchResponse response = movieRaterService.searchImdbMovies(query);
        return ResponseEntity.ok(response);
    }

    @Operation(summary = "Get movie details from imdb", description = "Put imdbId to get full movie details (title, year, plot, image) from IMDB")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Detalji filma"),
            @ApiResponse(responseCode = "500", description = "Greska pri dohvatanju filma",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @GetMapping("/imdb/{imdbId}")
    public ResponseEntity<ImdbTitleResponse> getMovieDetails(
            @Parameter(description = "ID from imdb") @PathVariable(value = "imdbId") String imdbId) {
        ImdbTitleResponse response = movieRaterService.getImdbMovieById(imdbId);
        return ResponseEntity.ok(response);
    }

    @Operation(summary = "Add review", description = "Save a movie and add a review with rating (1-10) and comment")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Recenzija uspesno sacuvana"),
            @ApiResponse(responseCode = "400", description = "Nevalidan zahtev",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class))),
            @ApiResponse(responseCode = "500", description = "Greska pri cuvanju",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @PostMapping("/review")
    public ResponseEntity<String> addReview(@Valid @RequestBody ReviewRequest request) {
        movieRaterService.saveMovieAndReview(request.getReview(), request.getMovie()).join();
        return ResponseEntity.ok("Recenzija je uspesno sacuvana!");
    }

    @Operation(summary = "Get reviews for a movie", description = "Put imdbId to see all reviews and ratings for that movie")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Lista recenzija"),
            @ApiResponse(responseCode = "500", description = "Greska pri dohvatanju",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @GetMapping("/{imdbId}/reviews")
    public ResponseEntity<List<Review>> getReviews(
            @Parameter(description = "ID from imdb") @PathVariable(value = "imdbId") String imdbId) {
        List<Review> reviews = movieRaterService.getMovieReviews(imdbId).join();
        return ResponseEntity.ok(reviews);
    }

    @Operation(summary = "Get all reviews for a user", description = "Put userId to see all movies/reviews added by that user (collection)")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Lista recenzija korisnika"),
            @ApiResponse(responseCode = "500", description = "Greska pri dohvatanju",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @GetMapping
    public ResponseEntity<List<Review>> getUserReviews(
            @Parameter(description = "ID of the user") @RequestParam(value = "userId") String userId) {
        List<Review> reviews = movieRaterService.getReviewsForUser(userId).join();
        return ResponseEntity.ok(reviews);
    }

    @Operation(summary = "Delete review", description = "Put reviewId to delete a review from the user's collection")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Recenzija uspesno obrisana"),
            @ApiResponse(responseCode = "500", description = "Greska pri brisanju",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @DeleteMapping("/review/{reviewId}")
    public ResponseEntity<String> deleteReview(
            @Parameter(description = "ID of the review") @PathVariable(value = "reviewId") String reviewId) {
        movieRaterService.deleteReview(reviewId).join();
        return ResponseEntity.ok("Recenzija je uspesno obrisana!");
    }
}
