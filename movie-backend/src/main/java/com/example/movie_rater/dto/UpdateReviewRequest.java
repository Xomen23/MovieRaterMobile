package com.example.movie_rater.dto;

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
@Schema(description = "Zahtev za azuriranje postojece recenzije")
public class UpdateReviewRequest {

    @NotBlank(message = "User ID je obavezan")
    @Schema(description = "ID korisnika koji menja recenziju (mora biti vlasnik)")
    private String userId;

    @NotNull(message = "Ocena je obavezna")
    @Min(value = 1, message = "Ocena mora biti izmedju 1 i 10")
    @Max(value = 10, message = "Ocena mora biti izmedju 1 i 10")
    @Schema(description = "Nova ocena filma (1-10)", example = "8")
    private Integer rating;

    @Schema(description = "Novi tekst recenzije")
    private String reviewText;
}
