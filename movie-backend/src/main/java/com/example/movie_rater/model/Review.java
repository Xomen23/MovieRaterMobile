package com.example.movie_rater.model;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Review {
    private String id;
    private String userId;
    private String username;
    private String imdbId;
    private Integer rating;
    private String reviewText;
    private String createdAt;
}