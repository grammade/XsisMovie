namespace XsisMovie.Test;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FakeItEasy;
using FluentValidation;
using PaginationHelper;
using XsisMovie.Common.Dtos;
using XsisMovie.Controllers;
using XsisMovie.Entities;
using XsisMovie.Persistence;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FluentValidation.Results;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Sockets;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

public class MovieTest {
    private Mock<IContext> _context;
    private Mock<IValidator<MovieModifyDto>> _validator;
    private Mock<IMapper> _mapper;
    private Mock<IPageHelper> _pageHelper;

    private IQueryable<Movie> _movies;
    private MovieDto[] _moviesDto;
    private string _name;
    public MovieTest() => Arrange();

    [Fact]
    public async Task GetMovies_ReturnCorrentAmount() { 
        // Arrange
        var paginationDto = new PaginationDto(); 
        var controller = new MoviesController(_context.Object, _validator.Object, _mapper.Object, _pageHelper.Object);

        // Act
        var result = await controller.getMovies(paginationDto, "", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var data = okResult.Value;
        var envelope = Assert.IsType<Envelope<MovieDto[]>>(data);
        Assert.Equal(envelope.Meta.Count, _movies.Count());
    }

    [Fact]
    public async Task GetMovies_ReturnCorrentAmount_AfterSearch() {
        // Arrange
        var paginationDto = new PaginationDto();
        _name = "new";
        SeedData();
        var controller = new MoviesController(_context.Object, _validator.Object, _mapper.Object, _pageHelper.Object);

        // Act
        var result = await controller.getMovies(paginationDto, _name, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var data = okResult.Value;
        var envelope = Assert.IsType<Envelope<MovieDto[]>>(data);
        Assert.Equal(envelope.Meta.Count, _movies.Where(m => m.Title.Contains(_name)).Count());
    }

    [Fact]
    public async Task GetDetail_ReturnMovieDetail() {
        //Arrange
        var id = _movies.First().Id;
        var controller = new MoviesController(_context.Object, _validator.Object, _mapper.Object, _pageHelper.Object);

        //Act
        var result = await controller.getDetail(id, CancellationToken.None);
        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var data = okResult.Value;
        var movie = Assert.IsType<MovieDto>(data);
        Assert.Equal(movie.Id, id);
    }

    [Fact]
    public async Task GetDetail_ReturnMovieNotFoundl() {
        //Arrange
        var id = int.MaxValue;
        var controller = new MoviesController(_context.Object, _validator.Object, _mapper.Object, _pageHelper.Object);

        //Act
        var result = await controller.getDetail(id, CancellationToken.None);
        //Assert
        var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Add_ReturnsMovie() {
        //Arrange
        var id = int.MaxValue;
        var controller = new MoviesController(_context.Object, _validator.Object, _mapper.Object, _pageHelper.Object);
        var movieModifyDto = new MovieModifyDto {
            Title = "new new movie",
            Rating = 5
        };
        var addedMovie = new Movie {
            Id =id,
            Title = movieModifyDto.Title,
            Rating = movieModifyDto.Rating
        };
        var entityEntryMock = new Mock<EntityEntry<Movie>>();
        entityEntryMock.Setup(e => e.Entity).Returns(addedMovie);

        _context.Setup(c => c.Movies.AddAsync(It.IsAny<Movie>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Movie movie, CancellationToken cancellationToken) => { return entityEntryMock.Object; });

        _context.Setup(c => c.SaveChangesAsync(default))
            .Callback(() => {
                addedMovie.Id = id;
            })
            .ReturnsAsync(1);

        //Act
        var result = await controller.add(movieModifyDto);

        //Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
    }


    private void Arrange() {
        SeedData();
        var moviesDbSetMock = new Mock<DbSet<Movie>>();
        moviesDbSetMock.As<IQueryable<Movie>>().Setup(m => m.Expression).Returns(_movies.Expression);
        moviesDbSetMock.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(_movies.ElementType);
        moviesDbSetMock.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(_movies.GetEnumerator());
        
        moviesDbSetMock.As<IDbAsyncEnumerable<Movie>>()
            .Setup(m => m.GetAsyncEnumerator())
            .Returns(new TestDbAsyncEnumerator<Movie>(_movies.GetEnumerator()));
        moviesDbSetMock.As<IQueryable<Movie>>()
            .Setup(m => m.Provider)
            .Returns(new TestDbAsyncQueryProvider<Movie>(_movies.Provider));
        var addedMovie = new Movie {
            Id = 4, // Choose an appropriate ID for your test case
            Title = ""
        };

        _context = new Mock<IContext>();
        _context.Setup(c => c.Movies).Returns(moviesDbSetMock.Object);

        _validator = new Mock<IValidator<MovieModifyDto>>();
        _validator.Setup(v => v.ValidateAsync(It.IsAny<MovieModifyDto>(), default))
            .ReturnsAsync(new ValidationResult()); // Assuming validation is successful
        _mapper = new Mock<IMapper>();
        _mapper.Setup(m => m.ConfigurationProvider).Returns(() => new MapperConfiguration(cfg => {
            cfg.CreateMap<Movie, MovieDto>();
        }));
        _mapper.Setup(m => m.Map<MovieDto>(It.IsAny<Movie>()))
            .Returns((Movie source) => new MovieDto {
                Id = source.Id,
                Title = source.Title,
                Description = source.Description,
                Image = source.Image,
                Rating = source.Rating
            });
    }

    private void SeedData() {
        _movies = new List<Movie>
                {
            new Movie { Id = 1, Title = "movie1" },
            new Movie { Id = 2, Title = "movie2" },
            new Movie { Id = 3, Title = "new movie" }
        }.AsQueryable();
        _moviesDto = new MovieDto[] {
            new MovieDto { Id = 1, Title = "movie1" },
            new MovieDto { Id = 2, Title = "movie2" },
            new MovieDto { Id = 2, Title = "new movie" }
        };
        _pageHelper = new Mock<IPageHelper>();
        _pageHelper
            .Setup(ph => ph.GetPageAsync<MovieDto>(It.IsAny<IQueryable<MovieDto>>(), It.IsAny<PaginationDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IQueryable<MovieDto> items, PaginationDto paginationDto, CancellationToken cancellationToken) => {
                var data = _name is null ? _moviesDto : _moviesDto.Where(m => m.Title.Contains(_name)).ToArray();
                var metadata = new Meta {
                    Links = new Pagination(),
                    Count = data.Length
                };
                return new Envelope<MovieDto[]> {
                    Data = data,
                    Meta = metadata
                };
            });
    }
}