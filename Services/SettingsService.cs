using Microsoft.Extensions.Logging;
using Planilla_RRHH.Data.Repositories;
using Planilla_RRHH.Models;
using Planilla_RRHH.Services.Interfaces;

namespace Planilla_RRHH.Services;

public sealed class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _repository;
    private readonly ILogger<SettingsService>? _logger;
    
    public SettingsService(ISettingsRepository repository, ILogger<SettingsService>? logger = null)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public Task<RestaurantSettings> GetAsync() => _repository.GetAsync();
    
    public async Task<(bool success, string? error)> UpdateAsync(RestaurantSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        
        var updated = await _repository.UpdateAsync(settings);
        if (!updated)
            return (false, "Error al guardar la configuración");
        
        _logger?.LogInformation("Restaurant settings updated");
        return (true, null);
    }
}