package com.example.movie_rater.entity;

import jakarta.persistence.*;
import lombok.Data;

@Data
@Entity
@Table(name = "movie")
public class MovieEntity {

    @Id
    @Column(name = "id")
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Integer id;

    @Column(name = "image_url")
    private String imageUrl;

    @Column(name = "title")
    private String title;

    @Column(name = "imdb_id", unique = true)
    private String imdbId;

    @Column(name = "description")
    private String description;

    @Column(name = "movie_year")
    private Integer year;
}