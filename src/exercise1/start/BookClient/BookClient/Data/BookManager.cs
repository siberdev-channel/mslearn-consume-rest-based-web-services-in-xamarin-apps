using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.

namespace BookClient.Data
{
    public class BookManager
    {
        private const string Url = "http://bookserver2914.azurewebsites.net/api/books/";
        private string authorizationKey;

        private async Task<HttpClient> GetClient()
        {
            var client = new HttpClient();
            if (string.IsNullOrEmpty(authorizationKey))
            {
                authorizationKey = await client.GetStringAsync(Url + "login");
                authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
            }

            client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }

        public async Task<IEnumerable<Book>> GetAll()
        {
            var client = await GetClient();
            string result = await client.GetStringAsync(Url);
            return JsonConvert.DeserializeObject<IEnumerable<Book>>(result);
        }

        public async Task<Book> Add(string title, string author, string genre)
        {
            var book = new Book()
            {
                Title = title,
                Authors = new List<string>(new[] { author }),
                ISBN = string.Empty,
                Genre = genre,
                PublishDate = DateTime.Now.Date,
            };

            var client = await GetClient();
            var response = await client.PostAsync(Url,
               new StringContent(
                     JsonConvert.SerializeObject(book),
                     Encoding.UTF8, "application/json"));

            return JsonConvert.DeserializeObject<Book>(
               await response.Content.ReadAsStringAsync());
        }

        public async Task Update(Book book)
        {
            var client = await GetClient();
            await client.PutAsync(Url + "/" + book.ISBN,
               new StringContent(
                     JsonConvert.SerializeObject(book),
                     Encoding.UTF8, "application/json"));
        }

        public async Task Delete(string isbn)
        {
            var client = await GetClient();
            await client.DeleteAsync(Url + isbn);
        }
    }
}

#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.
