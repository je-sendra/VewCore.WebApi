using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace VewTech.VewCore.WebApi;

/// <summary>
/// A Controller extension that provides default CRUD operation methods using EntityFrameworkCore.
/// </summary>
/// <typeparam name="T">The model your controller will perform CRUD operations on. As for current limitations, the key type must be a Guid.</typeparam>
/// <param name="dbContext">The DbContext the controller will perform operations on.</param>
/// <param name="entities">The DbSet the controller will perform operations</param>
public class CrudController<T>(DbContext dbContext, DbSet<T> entities) : Controller where T : class
{
    /// <summary>
    /// Gets all the resources.
    /// </summary>
    /// <returns>A list with all the resources.</returns>
    [HttpGet]
    public virtual ActionResult<IEnumerable<T>> Get()
    {
        return entities;
    }

    /// <summary>
    /// Creates a new resource.
    /// </summary>
    /// <param name="resource">The resource to be created.</param>
    /// <returns>The newly created resource.</returns>
    [HttpPost]
    public virtual ActionResult<T> Post([FromBody] T resource)
    {
        entities.Add(resource);
        dbContext.SaveChanges();
        return Created("", resource);
    }

    /// <summary>
    /// Get a specific resource by its id.
    /// </summary>
    /// <param name="id">The id to search.</param>
    /// <returns>The resource with the specified id.</returns>
    [HttpGet("{id}")]
    public virtual ActionResult<T> GetById(Guid id)
    {
        var resource = entities.Find(id);
        if (resource == null) return NotFound();
        return Ok(resource);
    }

    /// <summary>
    /// Updates a resource by its id.
    /// </summary>
    /// <param name="id">The id of the resource to update.</param>
    /// <param name="resourcePatch">The resource patch.</param>
    /// <returns>The updated resource.</returns>
    [HttpPatch("{id}")]
    public virtual ActionResult<T> Patch(Guid id, [FromBody] JsonPatchDocument<T> resourcePatch)
    {
        var resource = entities.Find(id);
        if (resource == null) return NotFound();
        resourcePatch.ApplyTo(resource);
        dbContext.SaveChanges();
        return resource;
    }

    /// <summary>
    /// Deletes the resource with the specified id.
    /// </summary>
    /// <param name="id">The id of the resource to delete.</param>
    /// <returns>The action result.</returns>
    [HttpDelete("{id}")]
    public virtual ActionResult Delete(Guid id)
    {
        var resource = entities.Find(id);
        if (resource == null) return NotFound();
        entities.Remove(resource);
        dbContext.SaveChanges();
        return Ok();
    }
}