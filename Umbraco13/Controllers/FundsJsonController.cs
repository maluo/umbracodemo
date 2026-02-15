using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Models;
using System.Text.Json;

namespace Umbraco13.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class FundsJsonController : Controller
{
    private readonly IMediaService _mediaService;
    private readonly ILogger<FundsJsonController> _logger;

    public FundsJsonController(IMediaService mediaService, ILogger<FundsJsonController> logger)
    {
        _mediaService = mediaService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetFundsJson()
    {
        try
        {
            // Find the funds.json file in media library
            var mediaItem = FindMediaByNameRecursive("funds.json");
            if (mediaItem == null)
            {
                _logger.LogWarning("funds.json not found in media library");
                return NotFound(new { error = "funds.json not found" });
            }

            // Get file path from media item
            var physicalPath = mediaItem.GetValue<string>("path");
            var fullPath = !string.IsNullOrEmpty(physicalPath)
                ? Path.IsPathRooted(physicalPath)
                    ? System.IO.Directory.Exists(physicalPath)
                        ? Path.Combine(physicalPath, mediaItem.Name ?? "funds.json")
                        : physicalPath
                    : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", physicalPath.TrimStart('/'))
                : mediaItem.GetValue<string>("umbracoFile");

            if (!System.IO.File.Exists(fullPath))
            {
                _logger.LogWarning("File does not exist: {FilePath}", fullPath);
                return NotFound(new { error = "File not found on disk" });
            }

            // Read and return JSON content
            var jsonContent = await System.IO.File.ReadAllTextAsync(fullPath);
            return Content(jsonContent, "application/json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading funds.json");
            return StatusCode(500, new { error = "Error loading file", message = ex.Message });
        }
    }

    private IMedia? FindMediaByNameRecursive(string fileName)
    {
        try
        {
            var rootMedia = _mediaService.GetRootMedia();
            foreach (var root in rootMedia)
            {
                if (root == null) continue;

                var result = SearchMediaRecursively(root, fileName);
                if (result != null)
                    return result;
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    private IMedia? SearchMediaRecursively(IMedia media, string fileName)
    {
        if (media.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase))
        {
            return media;
        }

        try
        {
            var children = _mediaService.GetPagedChildren(media.Id, 0, 1000, out var total);
            if (total == 0 || children == null)
                return null;

            foreach (var child in children)
            {
                if (child == null) continue;

                var result = SearchMediaRecursively(child, fileName);
                if (result != null)
                    return result;
            }
        }
        catch
        {
            // Ignore errors during recursive search
        }

        return null;
    }
}
