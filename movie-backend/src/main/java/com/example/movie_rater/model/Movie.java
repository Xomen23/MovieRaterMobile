package com.example.movie_rater.model;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class Movie {
    private String id;
    private String title;
    private String imageUrl;
    private String description;
    private Integer year;
}