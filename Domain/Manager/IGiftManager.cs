using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.Dtos;

namespace Domain.Manager
{
    public interface IGiftManager
    {
        Task<GiftDto> GetRandomGiftAsync(string ci);
    }
}
