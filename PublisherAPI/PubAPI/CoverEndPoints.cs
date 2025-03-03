using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OpenApi;
using PublisherData;
using PublisherDomain;
namespace PubAPI;

public static class CoverEndpoints
{
    public static void MapCoverEndpoints (this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/Cover").WithTags(nameof(Cover));

        group.MapGet("/", async (PubContext db) =>
        {
            return await db.Covers.AsNoTracking().ToListAsync();
        })
        .WithName("GetAllCovers")
        .WithOpenApi();

        group.MapGet("/{CoverId}", async Task<Results<Ok<Cover>, NotFound>> (int coverid, PubContext db) =>
        {
            return await db.Covers.AsNoTracking()
                .FirstOrDefaultAsync(model => model.CoverId == coverid)
                is Cover model
                    ? TypedResults.Ok(model)
                    : TypedResults.NotFound();
        })
        .WithName("GetCoverById")
        .WithOpenApi();

        group.MapPut("/{CoverId}", async Task<Results<Ok, NotFound>> (int coverid, Cover cover, PubContext db) =>
        {
            var affected = await db.Covers
                .Where(model => model.CoverId == coverid)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(m => m.CoverId, cover.CoverId)
                    .SetProperty(m => m.DesignIdeas, cover.DesignIdeas)
                    .SetProperty(m => m.DigitalOnly, cover.DigitalOnly)
                    .SetProperty(m => m.BookId, cover.BookId)
                    );
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("UpdateCover")
        .WithOpenApi();

        group.MapPost("/", async (Cover cover, PubContext db) =>
        {
            db.Covers.Add(cover);
            await db.SaveChangesAsync();
            return TypedResults.Created($"/api/Cover/{cover.CoverId}",cover);
        })
        .WithName("CreateCover")
        .WithOpenApi();

        group.MapDelete("/{CoverId}", async Task<Results<Ok, NotFound>> (int coverid, PubContext db) =>
        {
            var affected = await db.Covers
                .Where(model => model.CoverId == coverid)
                .ExecuteDeleteAsync();
            return affected == 1 ? TypedResults.Ok() : TypedResults.NotFound();
        })
        .WithName("DeleteCover")
        .WithOpenApi();
    }
}