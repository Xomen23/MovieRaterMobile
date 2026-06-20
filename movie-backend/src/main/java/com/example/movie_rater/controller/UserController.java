package com.example.movie_rater.controller;

import com.example.movie_rater.dto.RegisterRequest;
import com.example.movie_rater.model.User;
import com.example.movie_rater.service.MovieRaterService;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.tags.Tag;
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
    @PostMapping("/register")
    public ResponseEntity<String> register(@RequestBody RegisterRequest request) {
        try {
            User user = new User();
            user.setFirstName(request.getFirstName());
            user.setLastName(request.getLastName());
            user.setUsername(request.getUsername());
            user.setPassword(request.getPassword());

            String userId = movieRaterService.registerUser(user).join();
            return ResponseEntity.status(HttpStatus.CREATED).body(userId);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.BAD_REQUEST).body(e.getMessage());
        }
    }

    @Operation(summary = "Login", description = "Put username and password to log in")
    @PostMapping("/login")
    public ResponseEntity<?> login(@Parameter(description = "Username") @RequestParam(value = "username") String username,
                                    @Parameter(description = "Password") @RequestParam(value = "password") String password) {
        try {
            User user = movieRaterService.loginUser(username, password).join();
            return ResponseEntity.ok(user);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.UNAUTHORIZED).body(e.getMessage());
        }
    }

    @Operation(summary = "Get user by username", description = "Put username to get the logged in user's data")
    @GetMapping("/me")
    public ResponseEntity<?> getLoggedInUser(@Parameter(description = "Username of the logged in user") @RequestParam(value = "username") String username) {
        try {
            User user = movieRaterService.getUserByUsername(username).join();
            return ResponseEntity.ok(user);
        } catch (Exception e) {
            return ResponseEntity.status(HttpStatus.NOT_FOUND).body(e.getMessage());
        }
    }
}
