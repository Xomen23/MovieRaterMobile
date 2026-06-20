package com.example.movie_rater;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.openfeign.EnableFeignClients;

@EnableFeignClients
@SpringBootApplication
public class MovieRaterApplication {

	public static void main(String[] args) {
		SpringApplication.run(MovieRaterApplication.class, args);
	}
}