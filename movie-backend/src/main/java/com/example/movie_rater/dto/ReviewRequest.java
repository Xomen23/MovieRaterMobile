package com.example.movie_rater.dto;

import com.example.movie_rater.model.Movie;
import com.example.movie_rater.model.Review;
import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.Valid;
import jakarta.validation.constraints.NotNull;
import lombok.Data;

@Data
@Schema(description = "Zahtev za dodavanje recenzije uz podatke o filmu")
public class ReviewRequest {

    @NotNull(message = "Recenzija je obavezna")
    @Valid
    @Schema(description = "Podaci o recenziji")
    private Review review;

    @NotNull(message = "Film je obavezan")
    @Valid
    @Schema(description = "Podaci o filmu")
    private Movie movie;
}
