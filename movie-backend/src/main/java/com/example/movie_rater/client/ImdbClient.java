package com.example.movie_rater.client;
import com.example.movie_rater.client.dto.ImdbSearchResponse;
import com.example.movie_rater.client.dto.ImdbTitleResponse;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.RequestParam;

@FeignClient(name = "ImdbClient", url = "https://api.imdbapi.dev")
public interface ImdbClient {

    @GetMapping("/search/titles")
    ImdbSearchResponse searchMovies(@RequestParam(value = "query") String query, @RequestParam(value = "limit") int limit);


    @GetMapping("/titles/{imdbId}")
    ImdbTitleResponse getMovieById(@PathVariable(value = "imdbId") String imdbId);


}
