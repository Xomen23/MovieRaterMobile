package com.example.movie_rater.client.dto;


import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@NoArgsConstructor
@AllArgsConstructor
public class ImdbTitle {

    private String id;

    private String originalTitle;

    private Integer startYear;

    private ImdbPrimaryImage primaryImage;


}
