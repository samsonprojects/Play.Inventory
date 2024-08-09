using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using static Play.Inventory.Service.Dtos;

namespace Play.Inventory.Service.Clients
{
    public class CatalogClient
    {
        private readonly HttpClient _httpClient;


        public CatalogClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }



        public async Task<IReadOnlyCollection<CatalogItemDto>> GetCatalogItemDtosAsync()
        {
            var items = await _httpClient.GetFromJsonAsync<IReadOnlyCollection<CatalogItemDto>>("/items");
            return items;

        }
    }
}