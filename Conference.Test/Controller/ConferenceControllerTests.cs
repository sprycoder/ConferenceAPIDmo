using Conference.API.Controllers;
using Conference.API.Model;
using Conference.API.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Conference.Test.Controller
{
    public class ConferenceControllerTests
    {
        private readonly Mock<IConferenceService> confService;
        private ConferenceController controller;

        public ConferenceControllerTests()
        {
            // Arrange
            confService = new Mock<IConferenceService>();
            confService.Setup(x => x.GetAllSessions()).Returns(Task.Run(() => (GetSessionResults().AsEnumerable())));
            confService.Setup(x => x.GetSessionById(It.Is<int>(i => i > int.MinValue))).Returns(Task.Run(() => GetSessionResults().First()));

            controller = new ConferenceController(confService.Object);
        }

        [Fact]
        public async void Get()
        {
            // Arrange
            var expected = GetSessionResults();

            // Act 
            var actionResult = await controller.Get();

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsAssignableFrom<IEnumerable<SessionResult>>(objectResult.Value);

            Assert.NotNull(result);
            Assert.Equal(expected.Count, result.Count());
            Assert.Equal(expected.First().SessionID, result.First().SessionID);
            Assert.Equal(expected.First().Title, result.First().Title);
            Assert.Equal(expected.First().Desciption, result.First().Desciption);
            Assert.Equal(expected.First().Speaker, result.First().Speaker);
            Assert.Equal(expected.First().TimeSlot, result.First().TimeSlot);
            Assert.Equal(expected.First().Href, result.First().Href);
            Assert.Equal(expected.First().Links.First().Href, result.First().Links.First().Href);
            Assert.Equal(expected.First().Links.First().Rel, result.First().Links.First().Rel);
        }

        [Fact]
        public async void GetSessionById_not_found()
        {
            // Act 
            var actionResult = await controller.GetSessionById(int.MinValue);

            // Assert
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(actionResult);
            var result = Assert.IsAssignableFrom<int>(notFoundObjectResult.Value);
            Assert.Equal(int.MinValue, result);
        }


        [Fact]
        public async void GetSessionById()
        {
            // Arrange
            var expected = GetSessionResults().First();

            // Act 
            var actionResult = await controller.GetSessionById(101);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(actionResult);
            var result = Assert.IsAssignableFrom<SessionResult>(okObjectResult.Value);

            Assert.Equal(expected.SessionID, result.SessionID);
            Assert.Equal(expected.Title, result.Title);
            Assert.Equal(expected.Desciption, result.Desciption);
            Assert.Equal(expected.Speaker, result.Speaker);
            Assert.Equal(expected.TimeSlot, result.TimeSlot);
            Assert.Equal(expected.Href, result.Href);
            Assert.Equal(expected.Links.First().Href, result.Links.First().Href);
            Assert.Equal(expected.Links.First().Rel, result.Links.First().Rel);
        }

        private List<SessionResult> GetSessionResults()
        {
            return new List<SessionResult> { new SessionResult {
                SessionID = 101,
                Title = "Test Session",
                Desciption = "Test Session description",
                Speaker = "Scott Guthrie",
                TimeSlot = DateTime.Today.ToString(),
                Href = "https://apiphany.azure-api.net/conference/session/101",
                Links = new List<LinkResult>
                {
                    new LinkResult
                    {
                        Rel = "http://tavis.net/rels/topics",
                        Href = "https://apiphany.azure-api.net/conference/session/100/topics"
                    }
                }
            } };
        }
    }
}
