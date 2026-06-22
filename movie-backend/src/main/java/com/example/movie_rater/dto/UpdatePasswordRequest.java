package com.example.movie_rater.dto;

import io.swagger.v3.oas.annotations.media.Schema;
import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Size;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Zahtev za promenu lozinke")
public class UpdatePasswordRequest {

    @NotBlank(message = "Trenutna sifra je obavezna")
    @Schema(description = "Trenutna lozinka", example = "secret123")
    private String currentPassword;

    @NotBlank(message = "Nova sifra je obavezna")
    @Size(min = 6, message = "Nova sifra mora imati najmanje 6 karaktera")
    @Schema(description = "Nova lozinka", example = "newsecret456")
    private String newPassword;
}
