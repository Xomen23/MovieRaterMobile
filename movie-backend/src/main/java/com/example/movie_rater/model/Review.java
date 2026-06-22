package com.example.movie_rater.model;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.Max;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Recenzija filma")
public class Review {

    @Schema(description = "ID recenzije (generise se automatski)")
    private String id;

    @NotBlank(message = "User ID je obavezan")
    @Schema(description = "ID korisnika koji ostavlja recenziju")
    private String userId;

    @NotBlank(message = "Username je obavezan")
    @Schema(description = "Username korisnika")
    private String username;

    @NotBlank(message = "IMDB ID je obavezan")
    @Schema(description = "IMDB ID filma", example = "tt1375666")
    private String imdbId;

    @NotNull(message = "Ocena je obavezna")
    @Min(value = 1, message = "Ocena mora biti izmedju 1 i 10")
    @Max(value = 10, message = "Ocena mora biti izmedju 1 i 10")
    @Schema(description = "Ocena filma (1-10)", example = "9")
    private Integer rating;

    @Schema(description = "Tekst recenzije")
    private String reviewText;

    @Schema(description = "Datum kreiranja recenzije")
    private String createdAt;
}
