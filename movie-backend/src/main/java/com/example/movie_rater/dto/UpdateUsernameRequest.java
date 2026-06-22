package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Zahtev za promenu korisnickog imena")
public class UpdateUsernameRequest {

    @NotBlank(message = "Novo korisnicko ime je obavezno")
    @Schema(description = "Novo korisnicko ime", example = "marko_novo")
    private String newUsername;
}
