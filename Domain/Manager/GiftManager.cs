using Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.External_Services;
using Serilog;
using Microsoft.Extensions.Logging;

namespace Domain.Manager
{
    public class GiftManager : IGiftManager
    {
        private readonly GiftStoreServices _externalService;
        private readonly ILogger<GiftManager> _logger;
        private static readonly Random _rnd = new();

        public GiftManager(
            GiftStoreServices externalService,
            ILogger<GiftManager> logger)
        {
            _externalService = externalService;
            _logger = logger;
        }

        public async Task<GiftDto?> GetRandomGiftAsync(string ci)
        {
            _logger.LogInformation("Obteniendo gift para CI={CI}", ci);

            List<GiftDto> gifts;
            try
            {
                gifts = await _externalService.GetAllGiftsItems();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar a la API externa de regalos");
                return null;
            }

            if (gifts == null || !gifts.Any())
            {
                _logger.LogWarning("La API devolvió 0 regalos");
                return null;
            }

            var picked = gifts[_rnd.Next(gifts.Count)];
            _logger.LogInformation("Regalo seleccionado ID={Id}, Name={Name}", picked.Id, picked.Name);
            return picked;

        }
    }
}
