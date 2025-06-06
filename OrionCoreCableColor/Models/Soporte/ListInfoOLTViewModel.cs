using OrionCoreCableColor.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.Models.Soporte
{
    public class ListInfoOLTViewModel : ListOLTViewModel
    {
        public List<ListInfoONUViewModel> ListInfoONU { get; set; }
        public bool? fbEstado{ get; set; }
    }
}