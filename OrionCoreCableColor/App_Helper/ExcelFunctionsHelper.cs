using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Excel.FinancialFunctions;


namespace OrionCoreCableColor.App_Helper
{
    public static class ExcelFunctionsHelper
    {
        public static decimal APR(decimal NPER, decimal PAGO, decimal VA)
        {
            try
            {
                var result = Financial.Rate((Double)NPER, (Double)(-PAGO), (Double)VA, 0, PaymentDue.EndOfPeriod);
                return Convert.ToDecimal(result * 12);
            }
            catch (Exception e)
            {
                return 0m;
            }


        }
    }
}