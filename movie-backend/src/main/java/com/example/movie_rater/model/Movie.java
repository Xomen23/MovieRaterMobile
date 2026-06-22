package com.example.movie_rater.model;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.NotNull;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Film")
public class Movie {

    @NotBlank(message = "ID filma je obavezan")
    @Schema(description = "IMDB ID filma", example = "tt1375666")
    private String id;

    @NotBlank(message = "Naslov filma je obavezan")
    @Schema(description = "Naslov filma", example = "Inception")
    private String title;

    @Schema(description = "URL slike postera")
    private String imageUrl;

    @Schema(description = "Opis filma")
    private String description;

    @NotNull(message = "Godina je obavezna")
    @Schema(description = "Godina izdavanja", example = "2010")
    private Integer year;
}
