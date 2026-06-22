package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import lombok.Data;

@Data
@Schema(description = "Zahtev za registraciju novog korisnika")
public class RegisterRequest {

    @NotBlank(message = "Ime je obavezno")
    @Schema(description = "Ime korisnika", example = "Marko")
    private String firstName;

    @NotBlank(message = "Prezime je obavezno")
    @Schema(description = "Prezime korisnika", example = "Markovic")
    private String lastName;

    @NotBlank(message = "Username je obavezan")
    @Size(min = 3, max = 30, message = "Username mora imati 3-30 karaktera")
    @Schema(description = "Korisnicko ime", example = "marko123")
    private String username;

    @NotBlank(message = "Sifra je obavezna")
    @Size(min = 6, message = "Sifra mora imati najmanje 6 karaktera")
    @Schema(description = "Lozinka korisnika", example = "secret123")
    private String password;
}
