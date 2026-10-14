using Planilla_RRHH.Models;

namespace Planilla_RRHH.Services.Interfaces;

public interface ISettingsService
{
    Task<RestaurantSettings> GetAsync();
    Task<(bool success, string? error)> UpdateAsync(RestaurantSettings settings);
}