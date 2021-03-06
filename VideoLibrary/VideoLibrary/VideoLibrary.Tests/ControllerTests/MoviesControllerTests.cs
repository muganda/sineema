﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using VideoLibrary.BusinessEntities.Models.Model;
using VideoLibrary.BusinessLogic.Repositories.ActorRepository;
using VideoLibrary.BusinessLogic.Repositories.MovieRepository;
using VideoLibrary.BusinessLogic.Services.ActorCrudService;
using VideoLibrary.BusinessLogic.Services.MovieCrudService;
using VideoLibrary.Controllers;

namespace VideoLibrary.Tests.ControllerTests
{
    [TestFixture]
    public class MoviesControllerTests
    {
        private Mock<IMovieRepository> _moqMovieRepository = null;
        private Mock<IActorRepository> _moqActorRepository = null;

        private MoviesController _moviesController;

        [SetUp]
        public void SetUp()
        {
            _moqMovieRepository = new Mock<IMovieRepository>();
            _moqActorRepository = new Mock<IActorRepository>();

            _moviesController = new MoviesController(new MovieService(_moqMovieRepository.Object), new ActorService(_moqActorRepository.Object));

            _moqMovieRepository.Setup(c => c.GetAll()).ReturnsAsync(new List<Movie>() { new Movie() { Id = 1, DateAdded = DateTime.Now, Genre = Genre.Kinigeria, Duration = 250, IsActive = true, Title = "Nigerian Movie" } });

        }

        [TearDown]
        public void TearDown()
        {
            _moqActorRepository = null;
            _moqMovieRepository = null;
        }

        [Test]
        public async Task Should_Call_GetMovies_Once()
        {
            //arrange...
            
            //act...
            await _moviesController.Index("","",1);

            //assert...
            _moqMovieRepository.Verify(x => x.GetAll(), Times.Once());
        }

        [Test]
        public async Task Should_Have_Detail_View()
        {
            //act...
            var result = (await _moviesController.Index("","",1)) as ViewResult;

            //assert...
            Assert.AreEqual(string.Empty, result.ViewName);
        }

        [Test]
        public async Task Reject_Invalid_Movie()
        {
            var movie = new Movie{ Title = "", LeadActorId = 1, Duration = 100, Genre = Genre.Christian, DateAdded = DateTime.Today};
            var result = (await _moqMovieRepository.Object.InsertAsync(movie));
            Assert.IsNull(result);
        }

        [Test]
        public async Task Accept_Valid_Movie()
        {
            var movie = new Movie { Title = "Yet another title", LeadActorId = 1, Duration = 100, Genre = Genre.Christian, DateAdded = DateTime.Today };
            var result = (await _moqMovieRepository.Object.InsertAsync(movie));
            Assert.IsNotNull(result);
        }

        public async Task Performs_Correct_Pagination()
        {
            var allMovies = (await _moqMovieRepository.Object.GetAll());
            var result = (await _moviesController.Index("", "", 1)) as ViewResult;
            //TODO
        }
    }
}
