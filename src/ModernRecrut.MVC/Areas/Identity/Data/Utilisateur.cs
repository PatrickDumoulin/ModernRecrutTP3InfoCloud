using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ModernRecrut.MVC.Areas.Identity.Data;

// Add profile data for application users by adding properties to the Utilisateur class

public enum TypeUtilisateur
{
    Employé = 1,
    Candidat = 2
}

public class Utilisateur : IdentityUser
{
    [DisplayName("Type d'utilisateur"), Required(ErrorMessage = "Veuillez renseigner le type d'utilisateur")]
    public TypeUtilisateur TypeUtilisateur {  get; set; }

    [DisplayName("Nom"), Required(ErrorMessage = "Veuillez renseigner le nom")]
    public string Nom {  get; set; }

    [DisplayName("Prénom"), Required(ErrorMessage = "Veuillez renseigner le prénom")]
    public string Prenom {  get; set; }

    [NotMapped]
    public string? NomComplet
    {
        get
        {
            return this.Nom + ", " + this.Prenom;
        }
    }
}

