package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Prosecna ocena filma")
public class AverageRatingResponse {

    @Schema(description = "IMDB ID filma", example = "tt1375666")
    private String imdbId;

    @Schema(description = "Prosecna ocena (1 decimala)", example = "8.5")
    private double averageRating;

    @Schema(description = "Broj recenzija", example = "12")
    private int reviewCount;
}
