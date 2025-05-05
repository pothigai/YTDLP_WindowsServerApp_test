namespace YTDLP_WindowsServerApp_test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.Write("Enter a search term: ");
            string searchTerm = Console.ReadLine();

            Search searcher = new Search();
            string selectedURL = await searcher.DisplayResults(searchTerm);

            if (!string.IsNullOrEmpty(selectedURL))
            {
                Console.Write($"This is your selected URL: {selectedURL}");
            }
            Console.ReadLine();
        }
    }
}
