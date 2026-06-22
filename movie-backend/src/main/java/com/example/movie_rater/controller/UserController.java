package com.example.movie_rater.controller;

import com.example.movie_rater.dto.ErrorResponse;
import com.example.movie_rater.dto.LoginRequest;
import com.example.movie_rater.dto.RegisterRequest;
import com.example.movie_rater.model.User;
import com.example.movie_rater.service.MovieRaterService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Content;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.responses.ApiResponse;
import io.swagger.v3.oas.annotations.responses.ApiResponses;
import io.swagger.v3.oas.annotations.tags.Tag;
import jakarta.validation.Valid;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

@Tag(name = "User Controller", description = "Controller for user registration, login and profile")
@RestController
@RequestMapping("/api/users")
@CrossOrigin(origins = "*")
public class UserController {

    private final MovieRaterService movieRaterService;

    public UserController(MovieRaterService movieRaterService) {
        this.movieRaterService = movieRaterService;
    }

    @Operation(summary = "Register", description = "Put register credentials to create a new user")
    @ApiResponses({
            @ApiResponse(responseCode = "201", description = "Korisnik uspesno registrovan"),
            @ApiResponse(responseCode = "400", description = "Nevalidan zahtev ili username vec postoji",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @PostMapping("/register")
    public ResponseEntity<String> register(@Valid @RequestBody RegisterRequest request) {
        User user = new User();
        user.setFirstName(request.getFirstName());
        user.setLastName(request.getLastName());
        user.setUsername(request.getUsername());
        user.setPassword(request.getPassword());

        String userId = movieRaterService.registerUser(user).join();
        return ResponseEntity.status(HttpStatus.CREATED).body(userId);
    }

    @Operation(summary = "Login", description = "Put username and password to log in")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Uspesna prijava"),
            @ApiResponse(responseCode = "401", description = "Pogresni kredencijali",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @PostMapping("/login")
    public ResponseEntity<User> login(@Valid @RequestBody LoginRequest request) {
        User user = movieRaterService.loginUser(request.getUsername(), request.getPassword()).join();
        return ResponseEntity.ok(user);
    }

    @Operation(summary = "Get user by username", description = "Put username to get the logged in user's data")
    @ApiResponses({
            @ApiResponse(responseCode = "200", description = "Korisnik pronadjen"),
            @ApiResponse(responseCode = "404", description = "Korisnik nije pronadjen",
                    content = @Content(schema = @Schema(implementation = ErrorResponse.class)))
    })
    @GetMapping("/me")
    public ResponseEntity<User> getLoggedInUser(
            @Parameter(description = "Username of the logged in user") @RequestParam(value = "username") String username) {
        User user = movieRaterService.getUserByUsername(username).join();
        return ResponseEntity.ok(user);
    }
}
