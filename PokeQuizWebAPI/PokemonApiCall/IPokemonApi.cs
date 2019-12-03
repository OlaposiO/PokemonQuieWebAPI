using PokeQuizWebAPI.Models.PokemonViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace PokeQuizWebAPI.PokemonApiCall
{
    public interface IPokemonApi
    {
        Task<AllPokemonInfo> GetPokemon(int id);
        //Task<AllPokemonInfo> GetAllPokemon(int id);
    }
}
