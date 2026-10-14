using Planilla_RRHH.Models;

namespace Planilla_RRHH.Data.Repositories;

public interface ISettingsRepository
{
    Task<RestaurantSettings> GetAsync();
    Task<bool> UpdateAsync(RestaurantSettings settings);
}