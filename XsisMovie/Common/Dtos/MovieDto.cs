using AutoMapper;
using XsisMovie.Common.Interfaces;
using XsisMovie.Entities;

namespace XsisMovie.Common.Dtos {
    public class MovieDto : IMapFrom<Movie> {
        public int Id { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public float? Rating { get; set; }
        public string? Image { get; set; }
        public void Mapping(Profile profile) {
            profile.CreateMap<Movie, MovieDto>().ReverseMap();
        }
    }
    public class MovieModifyDto : IMapFrom<Movie> {
        public required string Title { get; set; }
        public string? Description { get; set; }
        public float? Rating { get; set; }
        public string? Image { get; set; }
        public void Mapping(Profile profile) {
            profile.CreateMap<Movie, MovieModifyDto>().ReverseMap();
        }
    }
}
