package com.example.movie_rater.client.dto;

import lombok.AllArgsConstructor;
import lombok.Data;
import lombok.NoArgsConstructor;

import java.util.List;

@Data
@AllArgsConstructor
@NoArgsConstructor
public class ImdbSearchResponse {

    private List<ImdbTitle> titles;

}
