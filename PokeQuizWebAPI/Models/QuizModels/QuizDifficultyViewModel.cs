using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeQuizWebAPI.Models.QuizModels
{
    public class QuizDifficultyViewModel
    {
        public int[] AmountOfQuestions = new[] { 25, 50, 100 };
        public int SelectedNumberOfQuestions { get; set; }
    }
}
