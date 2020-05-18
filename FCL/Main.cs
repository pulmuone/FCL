using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FCL
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }


        private void btnForm1_Click(object sender, EventArgs e)
        {
            Form1 form = new Form1();
            form.MdiParent = MDIParent1.ActiveForm;
            form.Show();

        }
        private void btnForm2_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.MdiParent = MDIParent1.ActiveForm;
            form.Show();
        }


        private void btnForm3_Click(object sender, EventArgs e)
        {
            Form3 form = new Form3();
            form.MdiParent = MDIParent1.ActiveForm;
            form.Show();

        }
    }
}
