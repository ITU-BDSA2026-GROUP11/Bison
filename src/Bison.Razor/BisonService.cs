public record ObservationViewModel(string Author, string Message, string Timestamp);

public interface IObservationService
{
    // Accepts a page number - default is 1
    public List<ObservationViewModel> GetObservations(int page = 1);
    
    // Accepts a page number - default is 1
    public List<ObservationViewModel> GetObservationsFromAuthor(string author, int page = 1);
}