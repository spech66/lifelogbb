namespace LifelogBb.Models.StrengthTrainings
{
    public class StrengthTrainingGraphViewModel
    {
        public List<string> Exercises { get; set; } = new();
        public string? SelectedExercise { get; set; }
    }
}
