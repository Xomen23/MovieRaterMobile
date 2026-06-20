package com.example.movie_rater.client.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class ImdbTitleResponse {

    private String id;

    private String primaryTitle;

    private Integer startYear;

    private String plot;

    private ImdbPrimaryImage primaryImage;

}
