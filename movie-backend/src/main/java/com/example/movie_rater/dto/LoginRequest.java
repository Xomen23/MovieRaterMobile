package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import lombok.Data;

@Data
@Schema(description = "Zahtev za prijavu korisnika")
public class LoginRequest {

    @NotBlank(message = "Username je obavezan")
    @Schema(description = "Korisnicko ime", example = "marko123")
    private String username;

    @NotBlank(message = "Sifra je obavezna")
    @Schema(description = "Lozinka korisnika", example = "secret123")
    private String password;
}
