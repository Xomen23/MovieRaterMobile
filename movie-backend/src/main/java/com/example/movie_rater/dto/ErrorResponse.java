package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Standardni error odgovor API-ja")
public class ErrorResponse {

    @Schema(description = "Poruka o gresci", example = "Username vec postoji!")
    private String error;

    @Schema(description = "HTTP status kod", example = "400")
    private int status;
}
