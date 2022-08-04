using Newtonsoft.Json;
using RealWorldApp.Commons.Models.ArticleModel;
using System.Net;
using System.Text;

namespace RealWorldApp.Tests.IntegrationTests
{
    public class ArticleIntegrationTests : IntegrationTests
    {
        [Test]
        public async Task GetAllArticle_WithoutAnyArticle_ReturnEmptyResponse()
        {
            // Arrange

            // Act
            var response = await _httpClient.GetAsync("api/articles");
            // Assert 

            Assert.True(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task AddArticle_AuthorizedUserWithCorrectData_ReturnArticleResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            var response = await _httpClient.PostAsync("api/articles", new StringContent(
                JsonConvert.SerializeObject(new CreateUpdateArticleModelContainer()
                {
                    Article = new CreateUpdateArticleModel
                    {
                        Title = "title",
                        Description = "description",
                        Body = "body"
                    }
                }),
                Encoding.UTF8,
                "application/json")
            );

            // Assert 
            Assert.True(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task AddArticle_UnAuthorizedUser_ReturnArticleResponse()
        {
            // Arrange
            _httpClient.DefaultRequestHeaders.Authorization = null;

            // Act
            var response = await _httpClient.PostAsync("api/articles", new StringContent(
                JsonConvert.SerializeObject(new CreateUpdateArticleModelContainer()
                {
                    Article = new CreateUpdateArticleModel
                    {
                        Title = "title1",
                        Description = "description",
                        Body = "body"
                    }
                }),
                Encoding.UTF8,
                "application/json")
            );

            // Assert 
            Assert.True(!response.IsSuccessStatusCode);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task DeleteArticle_AuthorizedUser_ReturnArticleResponse()
        {
            // Arrange
            await AuthenticateAsync();

            // Act
            await _httpClient.PostAsync("api/articles", new StringContent(
                JsonConvert.SerializeObject(new CreateUpdateArticleModelContainer()
                {
                    Article = new CreateUpdateArticleModel
                    {
                        Title = "title",
                        Description = "description",
                        Body = "body"
                    }
                }),
                Encoding.UTF8,
                "application/json")
            );

            var response = await _httpClient.DeleteAsync("api/articles/title");

            // Assert 
            Assert.True(response.IsSuccessStatusCode);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        }
    }
}
