using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ModernRecrut.MVC.Areas.Identity.Data;
using ModernRecrut.MVC.Controllers;
using ModernRecrut.MVC.DTO;
using ModernRecrut.MVC.Interfaces;
using ModernRecrut.MVC.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRecrut.MVC.UnitTests.Controllers
{
    public class PostulationsControllerTests
    {
        [Fact]
        public async Task Postuler_CVAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            fixture.RepeatCount = 3;
            List<string> listDocuments = fixture.CreateMany<string>().ToList();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("CV");
            modelState["CV"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_LettreMotivationAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            fixture.RepeatCount = 3;
            //List<string> listDocuments = fixture.Build<string>().Without(s => s.StartsWith(candidatId + "_LettreDeMotivation_")).CreateMany().ToList(); 
            List<string> listDocuments = fixture.CreateMany<string>().ToList();
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("LettreMotivation");
            modelState["LettreMotivation"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Une lettre de motivation est obligatoire pour postuler. Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DatePasseeInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(-1), // Date dans le passé
                PretentionSalariale = 50000m
            };

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DateFutureInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(46), // Date dans le passé
                PretentionSalariale = 50000m
            };

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_DateAujourdhuiInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation();
            requetePostulation.CandidatId = candidatId;
            requetePostulation.OffreDemploiId = offreEmploi.Id;
            requetePostulation.DateDisponibilite = DateTime.Today; // Date dans le passé
            requetePostulation.PretentionSalariale = 50000m;
            
            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_PretentionSalarialHorsLimite_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 151000m
            };

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("PretentionSalariale");
            modelState["PretentionSalariale"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Votre présentation salariale est au-delà de nos limites");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }


        [Fact]
        public async Task Postuler_PostulationIsNull_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>(); 
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationService.Setup(p => p.Ajouter(requetePostulation)).ReturnsAsync((Postulation)null); // Postulation null
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requetePostulationResult = actionResult.Model as RequetePostulation;
            requetePostulationResult.Should().Be(requetePostulation);
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Once);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(requetePostulation.OffreDemploiId), Times.Once);
            
            //// Validations des erreurs dana le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("AjoutEchoue");
            modelState["AjoutEchoue"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Problème lors de l'ajout de la postulation, veuillez reessayer");

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
        }

        [Fact]
        public async Task Postuler_PostulationValide_Retourne_RedirectToAction()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Postulation postulationAjoutee = fixture.Create<Postulation>();
            mockPostulationService.Setup(p => p.Ajouter(requetePostulation)).ReturnsAsync(postulationAjoutee); // Postulation valide
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var redirectToActionResult = await postulationsController.Postuler(requetePostulation) as RedirectToActionResult;

            // Alors
            redirectToActionResult.Should().NotBeNull();
            redirectToActionResult.ControllerName.Should().Be("OffreEmplois");
            redirectToActionResult.ActionName.Should().Be("Index");
            mockPostulationService.Verify(p => p.Ajouter(It.IsAny<RequetePostulation>()), Times.Once());
        }

        [Fact]
        public async Task Postuler_OffreEmploiInexistant_Retourne_NotFound()
        {
            // Etant donné
            //// Fixtures
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();

            // requete
            RequetePostulation requetePostulation = new RequetePostulation
            {
                CandidatId = candidatId,
                OffreDemploiId = fixture.Create<int>(),
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation Instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync((OffreEmploi)null);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Postuler(requetePostulation);

            // Alors
            actionResult.Should().BeOfType(typeof(NotFoundResult));
        }

        [Fact]
        public async Task Edit_IdInexistant_Retourne_NotFound()
        {
            // Etant donné
            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService

            mockPostulationsService.Setup(p => p.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync((Postulation)null);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            //Lorsque
            var actionResult = await postulationsController.Edit(3);

            // Alors
            actionResult.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Edit_IdExistant_Retourne_ViewResult()
        {
            // Etant donné
            Fixture fixture = new Fixture();

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Postulation postulation = fixture.Create<Postulation>();
            postulation.DateDisponibilite = DateTime.Today.AddDays(1); 

            mockPostulationsService.Setup(p => p.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(postulation);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            //Lorsque
            var actionResult = await postulationsController.Edit(3) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull();
            Postulation postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().Be(postulation);

            //// ViewData
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_DateDisponibiliteInferieurA5jours_Retourne_ViewResultAvecAuthorisationModificationFalse()
        {
            // Etant donné
            Fixture fixture = new Fixture();

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Postulation postulation = fixture.Create<Postulation>();
            postulation.DateDisponibilite = DateTime.Today.AddDays(-6); 

            mockPostulationsService.Setup(p => p.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(postulation);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            //Lorsque
            var actionResult = await postulationsController.Edit(3) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull();
            Postulation postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().Be(postulation);

            //// ViewData
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(false);
        }

        [Fact]
        public async Task Edit_DateDisponibiliteSuperieurA5jours_Retourne_ViewResultAvecAuthorisationModificationFalse()
        {
            // Etant donné
            Fixture fixture = new Fixture();

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            Postulation postulation = fixture.Create<Postulation>();
            postulation.DateDisponibilite = DateTime.Today.AddDays(6); 

            mockPostulationsService.Setup(p => p.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(postulation);

            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            //Lorsque
            var actionResult = await postulationsController.Edit(3) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull();
            Postulation postulationResult = actionResult.Model as Postulation;
            postulationResult.Should().Be(postulation);

            //// ViewData
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(false);
        }

        [Fact]
        public async Task Edit_Post_PostulationValide_Retourne_RedirectToAction()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.Modifier(postulation)).Returns(Task.CompletedTask); // Postulation valide
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);


            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque 
            RedirectToActionResult redirectToActionResult = await postulationsController.Edit(postulation) as RedirectToActionResult;

            // Alors
            redirectToActionResult.Should().NotBeNull();
            redirectToActionResult.ActionName.Should().Be("ListePostulations");
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Once());
        }

        [Fact]
        public async Task Edit_Post_DateDisponibiliteInferieurA5jours_Retourne_ViewResultAvecAuthorisationModificationFalse()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document CV

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            Postulation postulationExistante = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(-6),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.Modifier(postulation)).Returns(Task.CompletedTask); // Postulation valide
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulationExistante.Id)).ReturnsAsync(postulationExistante); // Postulation valide
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);


            var postulationsController = new PostulationsController(mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque 
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);

            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeTrue();

            //// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(false);
        }

        [Fact]
        public async Task Edit_Post_CVAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("CV");
            modelState["CV"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Un CV est obligatoire pour postuler. Veuillez déposer au préalable un CV dans votre espace Documents");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_Post_LettreMotivationAbsent_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("LettreMotivation");
            modelState["LettreMotivation"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Une lettre de motivation est obligatoire pour postuler. Veuillez déposer au préalable une lettre de motivation dans votre espace Documents");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);

        }

        [Fact]
        public async Task Edit_Post_DatePasseeInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(-1),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_Post_DateFutureInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(46),
                PretentionSalariale = 50000m
            };

            Postulation postulationExistante = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today,
                PretentionSalariale = 50000m
            };
            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulationExistante.Id)).ReturnsAsync(postulationExistante); // Postulation valide
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_Post_DateAujourdhuiInvalide_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today,
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("DateDisponibilite");
            modelState["DateDisponibilite"].Errors.FirstOrDefault().ErrorMessage.Should().Be("La date de disponibilité doit être supérieure à la date du jour et inférieure au < date correspondante à date du jour + 45 jours >");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_Post_PretentionSalarialHorsLimite_Retourne_ViewResultAvecModelStateError()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 151000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            mockPostulationsService.Setup(p => p.ObtenirSelonId(postulation.Id)).ReturnsAsync(postulation); // Postulation existante
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync(offreEmploi);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation) as ViewResult;

            // Alors
            actionResult.Should().NotBeNull(); // Action Result Not Null
            var requeteEditResult = actionResult.Model as Postulation;
            requeteEditResult.Should().Be(postulation);
            mockPostulationsService.Verify(p => p.Modifier(It.IsAny<Postulation>()), Times.Never);
            mockOffreEmploiService.Verify(o => o.ObtenirSelonId(postulation.OffreDEmploiId), Times.Once);
            
            ////// Validations des erreurs dans le ModelState
            var modelState = postulationsController.ModelState;
            modelState.IsValid.Should().BeFalse();
            modelState.ErrorCount.Should().Be(1);
            modelState.Should().ContainKey("PretentionSalariale");
            modelState["PretentionSalariale"].Errors.FirstOrDefault().ErrorMessage.Should().Be("Votre présentation salariale est au-delà de nos limites");

            ////// ViewData
            actionResult.ViewData.Should().ContainKey("OffreEmploi").WhoseValue.Should().BeSameAs(offreEmploi);
            actionResult.ViewData.Should().ContainKey("modificationAuthorisee").WhoseValue.Should().Be(true);
        }

        [Fact]
        public async Task Edit_Post_OffreEmploiInexistant_Retourne_NotFound()
        {
            // Etant donné
            //// Fixture
            Fixture fixture = new Fixture();

            string candidatId = fixture.Create<string>();
            string valideLettreMotivation = candidatId + "_LettreDeMotivation_" + fixture.Create<string>();
            string valideCV = candidatId + "_CV_" + fixture.Create<string>();

            //// Fixture OffreEmploi
            OffreEmploi offreEmploi = fixture.Create<OffreEmploi>();

            //// Fixture Documents
            List<string> listDocuments = new List<string>();
            listDocuments.Add(valideLettreMotivation); // Ajout un document lettre de Motivation Valide
            listDocuments.Add(valideCV); // Ajout un document lettre de Motivation Valide

            // requete
            Postulation postulation = new Postulation
            {
                CandidatId = candidatId,
                OffreDEmploiId = offreEmploi.Id,
                DateDisponibilite = DateTime.Today.AddDays(1),
                PretentionSalariale = 50000m
            };

            //// Initialisation instance Mock
            Mock<ILogger<PostulationsController>> mockLogger = new Mock<ILogger<PostulationsController>>();  // Logger
            Mock<IPostulationsService> mockPostulationsService = new Mock<IPostulationsService>();  // Postulation
            Mock<IDocumentsService> mockDocumentsService = new Mock<IDocumentsService>();  // Documents
            mockDocumentsService.Setup(d => d.ObtenirSelonUtilisateurId(It.IsAny<string>())).ReturnsAsync(listDocuments);
            Mock<IOffreEmploisService> mockOffreEmploiService = new Mock<IOffreEmploisService>();  // OffreEmploiService
            mockOffreEmploiService.Setup(o => o.ObtenirSelonId(It.IsAny<int>())).ReturnsAsync((OffreEmploi)null);

            var postulationsController = new PostulationsController( mockLogger.Object, mockPostulationsService.Object, mockDocumentsService.Object, mockOffreEmploiService.Object);

            // Lorsque
            var actionResult = await postulationsController.Edit(postulation);

            // Alors
            actionResult.Should().BeOfType(typeof(NotFoundResult));
        }
    }
}
