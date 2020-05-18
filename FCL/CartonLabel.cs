using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;

namespace FCL
{
    public class CartonLabel
    {
        private ReportDocument _myReport;


        public CartonLabel(ReportDocument myReport)
        {
            _myReport = myReport;
            this._myReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.PaperA4;
            this._myReport.PrintOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
        }


        public void Print()
        {
            
        }
    }
}
