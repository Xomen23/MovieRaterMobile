package com.example.movie_rater.dto;

import com.example.movie_rater.model.Movie;
import com.example.movie_rater.model.Review;
import lombok.Data;

@Data
public class ReviewRequest {
    private Review review;
    private Movie movie;
}