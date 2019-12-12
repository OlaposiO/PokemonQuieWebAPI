﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PokeQuizWebAPI.CalculationsService;
using PokeQuizWebAPI.Models.QuizModels;
using PokeQuizWebAPI.PokemonServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PokeQuizWebAPI.Controllers
{
    public class QuizController : Controller
    {
        private readonly IPokemonService _pokemonService;
        private readonly IRandomizer _randomizer;
        private readonly ISession _session;
        private readonly IQuizCalculations _quizCalulations;
        private readonly IQuizFlow _quizFlow;
        private readonly IPokemonUserSQLService _pokemonUserSQLService;

        public QuizController
        (IPokemonService pokemonService,
         IRandomizer randomizer,
         IHttpContextAccessor httpContextAccessor,
         IQuizCalculations quizCalculations,
         IQuizFlow quizFlow,
         IPokemonUserSQLService pokemonUserSQLService)

        {
            _pokemonService = pokemonService;
            _randomizer = randomizer;
            _session = httpContextAccessor.HttpContext.Session;
            _quizCalulations = quizCalculations;
            _quizFlow = quizFlow;
            _pokemonUserSQLService = pokemonUserSQLService;


        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SelectQuizDifficulty()
        {
            var correctAnswers = 0;
            _session.SetInt32("amountCorrect", correctAnswers);

            var viewModel = new QuizDifficultyViewModel();
            return View(viewModel);
        }

        public async Task<IActionResult> QuizView(QuizDifficultyViewModel userEnteredQuestion) //feeding into eds
        {
            _session.SetInt32("questionsAttempted", userEnteredQuestion.SelectedNumberOfQuestions);
            userEnteredQuestion.SelectedNumberOfQuestions = userEnteredQuestion.SelectedNumberOfQuestions + 1;
            var quizModel = new QuizViewModel();
            if (quizModel.PokemonAnswers.Count == 0)
            {
                quizModel.PokemonAnswers = _randomizer.RandomizeListOfAnsweres(userEnteredQuestion.SelectedNumberOfQuestions);
            }
            var testString = _session.GetString("pokemonStack");

            if (testString != null)
            {
                quizModel.PokemonAnswers = JsonConvert.DeserializeObject<Stack<int>>(_session.GetString("pokemonStack"));
            }

            quizModel.CorrectPokemon = await _pokemonService.MapPokemonInfo(quizModel.PokemonAnswers.Peek());
            var listOfWrongAnswers = _randomizer.RandomizeAditionalPokemon(quizModel.PokemonAnswers.Peek(), 4);
            quizModel.WrongAnswer1 = await _pokemonService.MapPokemonInfo(listOfWrongAnswers[0]);
            quizModel.WrongAnswer2 = await _pokemonService.MapPokemonInfo(listOfWrongAnswers[1]);
            quizModel.WrongAnswer3 = await _pokemonService.MapPokemonInfo(listOfWrongAnswers[2]);
            quizModel.PokemonAnswers.Pop();
            var storeStackIntoString = JsonConvert.SerializeObject(quizModel.PokemonAnswers);
            _session.SetString("pokemonStack", storeStackIntoString);
            _session.SetString("pokemonAnswer", quizModel.CorrectPokemon.PokemonName);

            //randomize answers

            quizModel.QuizAnswers.Add(quizModel.CorrectPokemon);
            quizModel.QuizAnswers.Add(quizModel.WrongAnswer1);
            quizModel.QuizAnswers.Add(quizModel.WrongAnswer2);
            quizModel.QuizAnswers.Add(quizModel.WrongAnswer3);

            quizModel.QuizAnswers = _randomizer.RandomizePossibleAnswerOrder(quizModel.QuizAnswers);

            if (quizModel.PokemonAnswers.Count == 0)
            {
                return View("QuizResults");
            }

            return View(quizModel);
        }

        // public async Task<ActionResult> CheckAnswer(string pokemonName)
        public async Task<IActionResult> QuizView(QuizDifficultyViewModel userEnteredQuestion, string pokemonName) //feeding into eds
        {
            QuizViewModel quizModel = await _quizFlow.SetupQuiz(userEnteredQuestion, pokemonName);
            if (quizModel.PokemonAnswers.Count == 0)
            {
                var quizResults = new QuizAttemptResultsViewModel();
                quizResults.AmountCorrect = _quizFlow.QuestionsCorrect; /*_session.GetInt32("amountCorrect").GetValueOrDefault();*/
                quizResults.QuestionsAttempted = _quizFlow.TotalQuetions;
                quizResults.ScoreThisAttempt = _quizCalulations.CalculateCurrentAttemptScore(quizResults.AmountCorrect, quizResults.QuestionsAttempted);
                _session.Clear();

                return View("QuizResults", quizResults);
                _pokemonUserSQLService.CreatePokemonUserData(quizResults);
                return View("QuizResults", quizResults);
            }
            return View(quizModel);
        }
        public IActionResult QuizResults()
        {
            var quizResultModel = new QuizAttemptResultsViewModel();
            return View(quizResultModel);
        }

        public IActionResult SubmitPokemonId()
        {
            return View();
        }

        public async Task<IActionResult> GetPokemonDetails(int id)
        {
            var result = await _pokemonService.MapPokemonInfo(id);
            return View(result);
        }



    }
}