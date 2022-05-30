using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mcc.Series.Ui.Type;
using Mcc.Series.Controls.Enum;
using Mcc.Series.Common.Enum;
using Mcc.Series.DataBase;

namespace Mcc.Clinic.Common.TEST
{
    public partial class test05 : Mcc.Series.Ui.FormBase
    {
        #region - member variable -

        #endregion

        #region - constructor -
        public test05()
        {
            InitializeComponent();

            this.SetBaseButtonHide(new EnumBaseButtonChoose[] { EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF5
                                                              , EnumBaseButtonChoose.btnbaseF6
                                                              , EnumBaseButtonChoose.btnbaseF7
                                                              , EnumBaseButtonChoose.btnbaseF8
                                                              , EnumBaseButtonChoose.btnbaseF10 });

            this.Load += new EventHandler(test05_Load);
            this.FormClosing += new FormClosingEventHandler(test05_FormClosing);
        }
        
        #endregion

        #region - form events / SetInitialize -
        private void test05_Load(object sender, EventArgs e)
        {
            try
            {
                // To Do..
                this.SetInitialize();
                this.SetGridHeader();
                setCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void test05_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region - SetInitialize / SetGridHeader -
        private void SetInitialize()
        {
            // To Do..
            lblbasetitle.Text = "환자등록";
        }

        private void SetGridHeader()
        {
            grdPtnt.AddColumn("ptnt_no", "환자번호", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.AddColumn("ptnt_nm", "환자명", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.AddColumn("addr", "주소", 100, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.AddColumn("att_dept", "진료과", 50, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.SetGridHeader();
            //grdPtnt.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        }
        #endregion

        #region - top button events -
        protected override void btnbaseF5_Click()
        {
            base.btnbaseF5_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void btnbaseF6_Click()
        {
            base.btnbaseF6_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void btnbaseF7_Click()
        {
            base.btnbaseF7_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void btnbaseF8_Click()
        {
            base.btnbaseF8_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void btnbaseF9_Click()
        {
            base.btnbaseF9_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void btnbaseF10_Click()
        {
            base.btnbaseF10_Click();

            try
            {
                // To Do..
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void setCombo()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"select dept_cd, dept_nm from hz_mst_dept_test;";

            DataTable dt = this.FillDataSet(sMsg).Tables[0];

            if (dt.Rows.Count > 0)                                                    
            {
                cbxPtntDept.FillData(dt, "dept_nm", "dept_cd", AddingItemMode.None);                      
            }
            else
            {
                MessageBox.Show("환자에 대한 정보가 없습니다. 환자정보를 등록해주세요");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            InserData();
            GetData();
        }

        private void InserData()
        {            
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"INSERT INTO public.hz_mst_ptnt_test
                                 (ptnt_no, ptnt_nm, addr, att_dept)
                                 VALUES(@ptnt_no, @ptnt_nm, @addr, @att_dept);";

            msg.AddParameter("ptnt_no", txtPtntNo.Text);
            msg.AddParameter("ptnt_nm", txtPtntNm.Text);
            msg.AddParameter("addr", txtPtntAddr.Text);
            msg.AddParameter("att_dept", cbxPtntDept.SelectedValue.ToString());

            this.ExecuteNonQuery(msg);            
        }
        private void GetData()
        {
            DataTable dt = new DataTable();
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"select * from hz_mst_ptnt_test";
            dt = this.FillDataSet(msg).Tables[0];
            
            if(dt.Rows.Count > 0){
            grdPtnt.FillData(dt);
        }
    }
}
