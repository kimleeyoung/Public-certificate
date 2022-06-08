using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PublicCertificate
{
    public partial class Form1 : Form
    {
        DataBase data = new DataBase();

        public Form1()
        {
            InitializeComponent();

            this.btnDelete.Click += BtnDelete_Click;
            this.btnInsert.Click += BtnInsert_Click;
            this.btnSelect.Click += BtnSelect_Click;
        }

        private void BtnSelect_Click(object sender, EventArgs e)
        {
            data.SelectData();
        }

        private void BtnInsert_Click(object sender, EventArgs e)
        {
            data.InsertData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            data.DeleteData();
        }
    }

    struct DataBase
    {
        public void SelectData()
        {
            MessageBox.Show("데이터 조회");
        }

        public void InsertData()
        {
            MessageBox.Show("데이터 저장");
        }

        public void DeleteData()
        {
            MessageBox.Show("데이터 삭제");
        }
    }
}
