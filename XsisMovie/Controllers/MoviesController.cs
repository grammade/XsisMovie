using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaginationHelper;
using XsisMovie.Common.Dtos;
using XsisMovie.Entities;
using XsisMovie.Persistence;

namespace XsisMovie.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class MoviesController : ControllerBase {
    private readonly IContext _context;
    private readonly IValidator<MovieModifyDto> _validator;
    private readonly IMapper _mapper;
    private readonly IPageHelper _pageHelper;

    public MoviesController(IContext context,
        IValidator<MovieModifyDto> validator,
        IMapper mapper,
        IPageHelper pageHelper) {
        _context = context;
        _validator = validator;
        _mapper = mapper;
        _pageHelper = pageHelper;
    }
    [HttpPost("Exception")]
    public async Task<ActionResult> testException() {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<ActionResult<Envelope<MovieDto>>> getMovies(
        [FromQuery] PaginationDto paginationDto, 
        string? name,
        CancellationToken cancellationToken) {
        var query = _context.Movies
            .AsNoTracking();
        if (!string.IsNullOrEmpty(name))
            query = query.Where(f => f.Title.Contains(name));

        var res = await _pageHelper.GetPageAsync(query.ProjectTo<MovieDto>(_mapper.ConfigurationProvider), paginationDto);
        return Ok(res);
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<MovieDto>> getDetail([FromRoute]int id, CancellationToken cancellation) {
        var res = _context.Movies
            .FirstOrDefault(m => m.Id.Equals(id));

        if (res is null) return BadRequest("Movie not found");
        var test = _mapper.Map<MovieDto>(res);
        return Ok(_mapper.Map<MovieDto>(res));
    }


    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Movie>> add([FromBody] MovieModifyDto model) {
        var valRes = await _validator.ValidateAsync(model);
        if (!valRes.IsValid) return BadRequest(valRes.Errors);

        var res = await _context.Movies.AddAsync(_mapper.Map<Movie>(model));
        await _context.SaveChangesAsync();

        return Ok(res.Entity);
    }
    [HttpPatch("{id}")]
    [Authorize]
    public async Task<ActionResult<Movie>> update([FromRoute]int id, [FromBody] JsonPatchDocument<MovieModifyDto> patch) {
        var db = await _context.Movies.FirstOrDefaultAsync(m => m.Id.Equals(id));
        if (db is null) return BadRequest();
        var dbMapped = _mapper.Map<MovieModifyDto>(db);

        patch.ApplyTo(dbMapped, ModelState);
        var valRes = await _validator.ValidateAsync(dbMapped);
        if (!valRes.IsValid) return BadRequest(valRes.Errors);

        var updated = _mapper.Map<Movie>(dbMapped);
        updated.Id = id;
        updated.UpdatedAt = DateTime.Now;
        _context.Movies.Update(updated);
        await _context.SaveChangesAsync();

        return Ok(dbMapped);
    }
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> delete([FromRoute]int id) {
        var deleted = await _context.Movies
            .Where(m => m.Id.Equals(id))
            .ExecuteDeleteAsync();

        if (deleted > 0) return Ok($"{id} has been deleted");
        else return BadRequest();
    }
}
