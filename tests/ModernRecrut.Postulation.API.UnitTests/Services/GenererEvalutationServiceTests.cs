using FluentAssertions;
using ModernRecrut.Postulation.API.Interfaces;
using ModernRecrut.Postulation.API.Models;
using ModernRecrut.Postulation.API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModernRecrut.Postulation.API.UnitTests.Services
{
    public class GenererEvalutationServiceTests
    {
        [Fact]
        public void GenererEvaluation_PretentionSalarialeInferieurNorme_Retourne_NoteNoteDetailSalaireInferieur()
        {
            // Etant donné
            decimal pretentionSalariale = 15000m;

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire inférieur à la norme");
        }

        [Fact]
        public void GenererEvaluation_PretentionSalarialeProcheInferieurNorme_Retourne_NoteNoteDetailSalaireProcheInferieur()
        {
            // Etant donné
            decimal pretentionSalariale = 25000m;

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire proche mais inférieur à la norme");
        }

        [Fact]
        public void GenererEvaluation_PretentionSalarialeDansNorme_Retourne_NoteNoteDetailSalaireNorme()
        {
            // Etant donné
            decimal pretentionSalariale = 55000m;

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire dans la norme");
        }

        [Fact]
        public void GenererEvaluation_PretentionSalarialeProcheSuperieurNorme_Retourne_NoteNoteDetailSalaireProcheSuperieur()
        {
            // Etant donné
            decimal pretentionSalariale = 85000m;

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire proche mais supérieur à la norme");
        }

        [Fact]
        public void GenererEvaluation_PretentionSalarialeSuperieurNorme_Retourne_NoteNoteDetailSalaireSuperieur()
        {
            // Etant donné
            decimal pretentionSalariale = 125000m;

            GenererEvaluationService genererEvaluationService = new GenererEvaluationService();

            // Lorsque
            var note = genererEvaluationService.GenererEvaluation(pretentionSalariale);

            // Alors
            note.Should().NotBeNull();
            note.Should().BeOfType(typeof(Note));
            note.NomEmeteur.Should().Be("ApplicationPostulation");
            note.NoteDetail.Should().Be("Salaire supérieur à la norme");
        }
    }
}
