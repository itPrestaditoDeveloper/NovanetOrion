using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Usuario
{
    public class CrearUsuarioViewModel
    {
        public int fiIdUsuario { get; set; }
        [Required]
        [Display(Name = "Nombre")]
        public string fcPrimerNombre { get; set; }
        [Required]
        [Display(Name = "Segundo Nombre")]
        public string fcSegundoNombre { get; set; }
        [Required]
        [Display(Name = "Apelllido")]
        public string fcPrimerApellido { get; set; }

        [Required]
        [Display(Name = "Segundo Apellido")]
        public string fcSegundoApellido { get; set; }



        public string fcNombreCorto { get; set; }


        public Int16 fiIDPuesto { get; set; }


        public byte fiTipoUsuario { get; set; }


        public byte fiIDDepartamento { get; set; }


        public int fiIDJefeInmediato { get; set; }


        public string fcUsuarioDominio { get; set; }


        public byte fiIDDominioRed { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "El {0} tiene que ser al menos de {2} caracteres.", MinimumLength = 4)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string fcPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("fcPassword", ErrorMessage = "la contraseña y la confirmacion no son el mismo")]
        public string ConfirmPassword { get; set; }


        public string fcPasswordToken { get; set; }

        public DateTime? fdFechaUltimoCambiodePassword { get; set; }

        [Required]
        [Display(Name = "Correro Electronico")]
        public string fcBuzondeCorreo { get; set; }


        public byte fiIDDominioCorreo { get; set; }



        public string fcDireccionFisica { get; set; }


        [Required]
        [Display(Name = "Identidad")]
        public string fcDocumentoIdentificacion { get; set; }


        public DateTime fdFechaAlta { get; set; }


        public int fiIDUsuarioSolicitante { get; set; }


        public int fiIDUsuarioCreador { get; set; }


        public int fiEstado { get; set; }

        public DateTime? fdFechaBaja { get; set; }

        [Required]
        [Display(Name = "Telefono")]
        public string fcTelefonoMovil { get; set; }


        public string fcToken { get; set; }


        public string fcIdAspNetUser { get; set; }


        public string fcUrlImage { get; set; }


        [Required]
        [Display(Name = "Nombre de Usuario")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Rol de Usuario")]
        public int IdRol { get; set; }

        [Display(Name = "Rol")]
        public string NombreRol { get; set; }

        public bool fbTecnico { get; set; }

        [Required]
        [Display(Name = "Seleccione empresa")]
        public int? fiIDEmpresa { get; set; }
        [Display(Name = "Distribuidor")]
        public int? fiIDDistribuidor { get; set; }
        public string connectionId { get; set; }
    }
}