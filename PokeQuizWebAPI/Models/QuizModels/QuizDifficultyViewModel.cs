﻿namespace PokeQuizWebAPI.Models.QuizModels
{
    public class QuizDifficultyViewModel
    {
        public int[] AmountOfQuestions = new[] { 2, 5, 25, 50, 100 };
        public int SelectedNumberOfQuestions { get; set; }
    }
}
