using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OrionCoreCableColor.App_Helper.Response
{
    public class TokenResponse
    {
        public int status { get; set; }
        public string msg { get; set; }
        public tokenModel data { get; set; }
    }

    public class tokenModel
    {
        public string token { get; set; }

    }
}