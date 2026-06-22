package com.example.movie_rater.model;

import com.fasterxml.jackson.annotation.JsonIgnore;
import io.swagger.v3.oas.annotations.media.Schema;
import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Schema(description = "Korisnik aplikacije")
public class User {
    @Schema(description = "Jedinstveni ID korisnika")
    private String id;

    @Schema(description = "Ime korisnika", example = "Marko")
    private String firstName;

    @Schema(description = "Prezime korisnika", example = "Markovic")
    private String lastName;

    @Schema(description = "Korisnicko ime", example = "marko123")
    private String username;

    @JsonIgnore
    @Schema(hidden = true)
    private String password;
}
