namespace Data.Dtos.Tutorial
{
    public class TutorialUpdateRequest
    {
        public string Title { get; set; }
        
        public string DifficultLevel { get; set; }
        
        public string CompletionTime { get; set; }
        
        public string Instruction { get; set; }
        
        public string Material { get; set; }
        
        public decimal Price { get; set; }
    }
}