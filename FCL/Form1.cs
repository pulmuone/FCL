using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ExcelDataReader;
using FCL.DataSet;
using FCL.Printer;
using FCL.Properties;
using FCL.Report;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace FCL
{
    public partial class Form1 : Form
    {

        private ReportDocument _myReport = new ReportDocument();

        private System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();

        private CrystalDecisions.Shared.PrintLayoutSettings PrintLayout = new CrystalDecisions.Shared.PrintLayoutSettings();
        private System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            CartonDataSet cartonDataSet = new CartonDataSet();
            DataTable dt = new DataTable();
            DataRow dr;
            string mcid = string.Empty;

            BarcodeLibSingleton.Instance.Barcode.IncludeLabel = false;
            BarcodeLibSingleton.Instance.Barcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Bmp;

            printerSettings.PrinterName = PrinterSingleton.Instance.PrinterName;

            System.Drawing.Printing.PageSettings pageSettings = new System.Drawing.Printing.PageSettings(printerSettings);
            //pageSettings.Landscape = true; //작동 안함...차이 없음.....

            PrintLayout.FitHorizontalPages = true;
            PrintLayout.Scaling = PrintLayoutSettings.PrintScaling.DoNotScale;
            PrintLayout.Centered = true;
                        

            foreach (DataGridViewRow Rows in this.dataGridView1.Rows)
            {
                if (Rows.Cells["Print"].Value != null)
                {
                    if (Rows.Cells["Print"].Value.ToString() == "True")
                    {

                        if (Rows.Cells["ProdCode"].Value != null)
                        {
                            dr = cartonDataSet.Tables[0].NewRow();

                            dr["ProdName"] = string.IsNullOrEmpty(Rows.Cells["ProdName"].Value.ToString()) ? string.Empty : Rows.Cells["ProdName"].Value.ToString().Trim();
                            dr["SubCode"] = string.IsNullOrEmpty(Rows.Cells["SubCode"].Value.ToString()) ? string.Empty : Rows.Cells["SubCode"].Value.ToString().Trim();
                            dr["ProdColor"] = string.IsNullOrEmpty(Rows.Cells["ProdColor"].Value.ToString()) ? string.Empty : Rows.Cells["ProdColor"].Value.ToString().Trim();
                            dr["Price"] = string.IsNullOrEmpty(Rows.Cells["Price"].Value.ToString()) ? string.Empty : Rows.Cells["Price"].Value.ToString().Trim();
                            dr["Price2"] = string.IsNullOrEmpty(Rows.Cells["Price2"].Value.ToString()) ? string.Empty : Rows.Cells["Price2"].Value.ToString().Trim();
                            dr["ProdCode"] = string.IsNullOrEmpty(Rows.Cells["ProdCode"].Value.ToString()) ? string.Empty : Rows.Cells["ProdCode"].Value.ToString().Trim();
                            dr["SizeType"] = string.IsNullOrEmpty(Rows.Cells["SizeType"].Value.ToString()) ? string.Empty : Rows.Cells["SizeType"].Value.ToString().Trim();
                            dr["CountryOfOrigin"] = string.IsNullOrEmpty(Rows.Cells["CountryOfOrigin"].Value.ToString()) ? string.Empty : Rows.Cells["CountryOfOrigin"].Value.ToString().Trim();

                            mcid = string.IsNullOrEmpty(Rows.Cells["ProdCode"].Value.ToString()) ? string.Empty : Rows.Cells["ProdCode"].Value.ToString().Trim(); 

                            if (string.IsNullOrEmpty(mcid))
                            {
                                dr["ProdCodeBarCode"] = null;
                            }
                            else
                            {
                                BarcodeLibSingleton.Instance.Barcode.Encode(BarcodeLib.TYPE.CODE128, mcid, 800, 100);
                                dr["ProdCodeBarCode"] = string.IsNullOrEmpty(mcid) ? null : BarcodeLibSingleton.Instance.Barcode.Encoded_Image_Bytes;
                            }

                            cartonDataSet.Tables[0].Rows.Add(dr);
                        }
                    }
                }
            }

            PrintOptions printOptions = this._myReport.PrintOptions;
            printOptions.DissociatePageSizeAndPrinterPaperSize = false;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    printOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.DefaultPaperOrientation;
                    break;
                case 1:
                    printOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Portrait;
                    break;
                case 2:
                    printOptions.PaperOrientation = CrystalDecisions.Shared.PaperOrientation.Landscape;
                    break;
            }

            //printOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;

            _myReport.SetDataSource(cartonDataSet);
            this._myReport.PrintToPrinter(printerSettings, pageSettings, false, PrintLayout);
            //this._myReport.PrintToPrinter(1, false, 0, 0);
        }
        

        private void Form1_Load(object sender, EventArgs e)
        {
            _myReport = new CrystalReport1();

            //_myReport.PrintOptions.PaperSize = CrystalDecisions.Shared.PaperSize.DefaultPaperSize;

            if (string.IsNullOrEmpty(Settings.Default.PaperOrientation))
            {
                comboBox1.SelectedIndex = 0;
            }
            else if(Settings.Default.PaperOrientation.ToString() == "DefaultPaperOrientation" )
            {
                comboBox1.SelectedIndex = 0;
            }
            else if(Settings.Default.PaperOrientation.ToString() == "Portrait" )
            {
                comboBox1.SelectedIndex = 1;
            }
            else if (Settings.Default.PaperOrientation.ToString() == "Landscape")
            {
                comboBox1.SelectedIndex = 2;
            }

            //더미로 해 놓아야 Manual로 입력한 것을 Export할 수 있다.
            CartonDataSet ds = new CartonDataSet();
            ds.Tables[0].Columns.Remove("ProdCodeBarCode");
            //ds.Tables[0].Columns.Remove("MCIDImage");
            this.dataGridView1.DataSource = ds.Tables[0];

            //Printer 기본설정
            if (string.IsNullOrEmpty(Settings.Default.PrinterName)) //프로그램에서의 기본 프린터
            {
                Settings.Default.PrinterName = new PrinterSettings().PrinterName; //작업 컴퓨터의 Default 프린터로 출력하게 한다.
            }

            PrinterSingleton.Instance.PrinterName = Settings.Default.PrinterName;
            PrinterName(PrinterSingleton.Instance.PrinterName);

            //this.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Version myVersion;

            myVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            this.Text = myVersion.Major.ToString() + "." + myVersion.Minor + "." + myVersion.Build + "," + myVersion.Revision;
        }

        private void btnSelectPrinter_Click(object sender, EventArgs e)
        {
            PrinterSelect printerSelect = new PrinterSelect();
            printerSelect.PrinaterSelectEvent += new PrinaterSelectEventHandler(PrinterName);

            printerSelect.ShowDialog();            
        }

        private void PrinterName(string printerName)
        {
            this.lblPrinter.Text = printerName;
        }

        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();

            string fileName = string.Empty;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            openFileDialog1.Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls";
            openFileDialog1.Title = "Select a Cursor Excel";

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;

            }
            else
            {
                return;
            }


            try
            {
                using (FileStream stream = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read))
                {
                    using (IExcelDataReader excelReader = ExcelReaderFactory.CreateReader(stream))
                    {
                        //excelReader.IsFirstRowAsColumnNames = true;
                        dt = excelReader.AsDataSet().Tables[0];
                    }
                }

                Debug.WriteLine(dt.Columns.Count);

                if (dt != null || dt.Rows.Count > 0)
                {
                    this.dataGridView1.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Excel File Error", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            }
            finally
            {

            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                //saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                //saveFileDialog.InitialDirectory = saveFileDialog.InitialDirectory = "{Desktop}";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                saveFileDialog.Filter = "Excel File(*.xlsx)|*.xlsx";

                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    GenerateExcel((DataTable)this.dataGridView1.DataSource, saveFileDialog.FileName, "CartonLabel");
                }


                this.Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Default;
                MessageBox.Show(ex.Message, "Error - btnExcel_Click", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }


        private void GenerateExcel(DataTable dataToExcel, string fileName, string excelSheetName)
        {

            Debug.WriteLine(dataToExcel.Rows.Count.ToString());
            //string fileName = "ByteOfCode";
            //string currentDirectorypath = Environment.;
            //string finalFileNameWithPath = string.Empty;

            //fileName = string.Format("{0}_{1}", fileName, DateTime.Now.ToString("dd-MM-yyyy"));
            //finalFileNameWithPath = string.Format("{0}\\{1}.xlsx", currentDirectorypath, fileName);

            //같은 이름 저장 하게 할 것인가?
            //Delete existing file with same file name.
            //if (File.Exists(fileName))
            //    File.Delete(fileName);

            FileInfo newFile = new FileInfo(fileName);

            //Step 1 : Create object of ExcelPackage class and pass file path to constructor.
            using (var package = new ExcelPackage(newFile))
            {
                //Step 2 : Add a new worksheet to ExcelPackage object and give a suitable name
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(excelSheetName);

                //Step 3 : Start loading datatable form A1 cell of worksheet.
                worksheet.Cells["A1"].LoadFromDataTable(dataToExcel, true);
                worksheet.Cells[2, 4, dataToExcel.Rows.Count+1, 5].Style.Numberformat.Format = "@";
                worksheet.Cells[2, 10, dataToExcel.Rows.Count+1, 11].Style.Numberformat.Format = "@";
                worksheet.Cells[2, 14, dataToExcel.Rows.Count+1, 14].Style.Numberformat.Format = "@";

                //Step 4 : (Optional) Set the file properties like title, author and subject
                //package.Workbook.Properties.Title = @"This code is part of tutorials available at http://bytesofcode.hubpages.com";
                //package.Workbook.Properties.Author = "Bytes Of Code";
                //package.Workbook.Properties.Subject = @"Register here for more http://hubpages.com/_bytes/user/new/";

                //Step 5 : Save all changes to ExcelPackage object which will create Excel 2007 file.
                package.Save();

                MessageBox.Show(string.Format("File name '{0}' generated successfully.", fileName), "File generated successfully!", MessageBoxButtons.OK, MessageBoxIcon.None);
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridView gridView = sender as DataGridView;
            if (null != gridView)
            {
                foreach (DataGridViewRow r in gridView.Rows)
                {
                    gridView.Rows[r.Index].HeaderCell.Value = (r.Index + 1).ToString();
                }
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.RowCount > 0)
            {
                foreach (DataGridViewRow r in this.dataGridView1.Rows)
                {
                    dataGridView1.Rows[r.Index].Cells["Print"].Value = true;
                };
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.RowCount > 0)
            {
                for (int i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Cells["Print"].Value = false;
                }
            }
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            if (this.dataGridView1.RowCount > 0)
            {
                for (int i = 0; i < this.dataGridView1.RowCount; i++)
                {
                    if (Convert.ToBoolean(dataGridView1.Rows[i].Cells["Print"].Value) == true)
                    {
                        dataGridView1.Rows[i].Cells["Print"].Value = false;
                    }
                    else
                    {
                        dataGridView1.Rows[i].Cells["Print"].Value = true;
                    }
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex > 0)
            {
                dataGridView1.Columns[e.ColumnIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            double storeNumber;

            if (this.dataGridView1.Rows.Count > 1)
            {
                if (!double.TryParse(this.txtFindStore.Text, out storeNumber))
                {
                    MessageBox.Show("[" + this.txtFindStore.Text + "] is not number");
                    return;
                }

                FindRowNumber("Store", storeNumber);
            }
        }

        private void FindRowNumber(string coulumnName, double storeNumber)
        {
            int rowNum = 0;
            bool isExists = false;
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (Convert.ToDouble(row.Cells[coulumnName].Value).Equals(storeNumber))
                {
                    rowNum = row.Index;
                    isExists = true;
                    break;
                }
            }

            if (isExists)
            {
                dataGridView1.CurrentCell = dataGridView1.Rows[rowNum].Cells["Print"];
                dataGridView1.Rows[rowNum].Selected = true;
                dataGridView1.CurrentCell.Selected = true;
            }
            else
            {
                MessageBox.Show(storeNumber + " is not exists.");

                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    dataGridView1.Rows[rowNum].Selected = false;
                    dataGridView1.CurrentCell.Selected = false;
                }
            }
        }

        private void txtFindStore_KeyDown(object sender, KeyEventArgs e)
        {
            double storeNumber;

            if (e.KeyData == Keys.Enter && this.dataGridView1.Rows.Count > 0)
            {
                if (!double.TryParse(this.txtFindStore.Text, out storeNumber))
                {
                    MessageBox.Show("[" + this.txtFindStore.Text + "] is not number");
                    return;
                }

                FindRowNumber("Store", storeNumber);

                //dataGridView1.Rows[FindRowNumber("col_StoreID", storeNumber)].Selected = true;
                //dataGridView1.CurrentCell = dataGridView1.Rows[FindRowNumber("col_StoreID", storeNumber)].Cells["col_TranChk"];
                //dataGridView1.CurrentCell.Selected = true;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.PaperOrientation = comboBox1.SelectedItem.ToString();
            Settings.Default.Save();
        }
    }
}
