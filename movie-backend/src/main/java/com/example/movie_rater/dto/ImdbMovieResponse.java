package com.example.movie_rater.dto;

import lombok.Data;
import java.util.List;

@Data
public class ImdbMovieResponse {
    private String id;
    private String title;
    private String originalTitle;
    private String fullTitle;
    private String year;
    private String image;
    private String plot;
    private String imDbRating;
}