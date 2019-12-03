using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PokeQuizWebAPI.Models.PokemonViewModels;

namespace PokeQuizWebAPI.PokemonApiCall
{
    public class PokemonApi : IPokemonApi
    {
        public async Task<AllPokemonInfo> GetPokemon(int id)
        {
            using (var httpClient = new HttpClient { BaseAddress = new Uri("https://pokeapi.co") })
            {
                
                var json = await httpClient.GetStringAsync($"/api/v2/pokemon-form/{id}");


                return JsonConvert.DeserializeObject<AllPokemonInfo>(json);


            }
        }
          
    }
}

