using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;

namespace YTDLP_WindowsServerApp_test
{
    public class Search
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> DisplayResults(string query)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = "API_KEY",
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = 50;

            try
            {
                var searchListResponse = await searchListRequest.ExecuteAsync();

                List<string> videoIds = new List<string>();
                int index = 1;

                Console.WriteLine("Search Results:");

                foreach (var searchResult in searchListResponse.Items)
                {
                    if (searchResult.Id.Kind == "youtube#video")
                    {
                        Console.WriteLine($"{index}. {searchResult.Snippet.Title}");
                        videoIds.Add(searchResult.Id.VideoId);
                        index++;
                    }
                }

                if (videoIds.Count == 0)
                {
                    Console.WriteLine("No video results found.");
                    return null;
                }

                Console.Write("\nEnter the number of the video you want to select: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex > 0 && selectedIndex <= videoIds.Count)
                {
                    string selectedVideoId = videoIds[selectedIndex - 1];
                    string streamUrl = await GetStreamUrlFromServer(selectedVideoId);
                    return streamUrl;
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                    return null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occurred: {e.Message}");
                return null;
            }
        }

        private async Task<string> GetStreamUrlFromServer(string videoId)
        {
            try
            {
                var jsonPayload = new StringContent(
                    JsonSerializer.Serialize(new { videoId }),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("http://localhost:8080/", jsonPayload);
                string responseJson = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseJson);
                if (doc.RootElement.TryGetProperty("url", out JsonElement urlElement))
                {
                    return urlElement.GetString();
                }

                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error communicating with server: {e.Message}");
                return null;
            }
        }
    }
}
