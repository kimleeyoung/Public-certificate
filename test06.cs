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
    public partial class test06 : Mcc.Series.Ui.FormBase
    {
        #region - member variable -

        #endregion

        #region - constructor -
        public test06()
        {
            InitializeComponent();

            this.SetBaseButtonHide(new EnumBaseButtonChoose[] { EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF10
                                                              , EnumBaseButtonChoose.btnbaseF5
                                                              , EnumBaseButtonChoose.btnbaseF6
                                                              , EnumBaseButtonChoose.btnbaseF7
                                                              , EnumBaseButtonChoose.btnbaseF8
                                                              , EnumBaseButtonChoose.btnbaseF10 });
            this.Text = "환자정보";
            this.Load += new EventHandler(test06_Load);
            this.FormClosing += new FormClosingEventHandler(test06_FormClosing);

            this.grdReceipt.DoubleClickRow += GrdReceipt_DoubleClickRow;
            this.btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!lblPtntInfo.Text.Equals("환자정보:") && !txtSymp.Text.Equals(""))
            {
                string[] str = lblPtntInfo.Text.Split('-');

                DBMessage msg = new DBMessage();
                msg.SqlStatement = @"UPDATE public.h1opdin_test SET symp_txt=@symp_txt WHERE recept_no=@recept_no;";

                msg.AddParameter("symp_txt", txtSymp.Text);
                msg.AddParameter("recept_no", str[1].Substring(0, 1).ToString());
                this.ExecuteNonQuery(msg);
            }
        }
        #endregion

        #region - form events / SetInitialize -
        private void test06_Load(object sender, EventArgs e)
        {
            try
            {
                // To Do..
                this.SetInitialize();
                this.SetGridHeader();
                GetData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void test06_FormClosing(object sender, FormClosingEventArgs e)
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
            lblbasetitle.Text = "진료화면";
        }

        private void SetGridHeader()
        {
            #region 접수대기 리스트
            grdReceipt.AddColumn("recept_no", "접수번호", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("ptnt_nm", "환자명", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("clinic_ymd", "진료일자", 100, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("clinic_time", "진료시간", 100, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.SetGridHeader();
            grdReceipt.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion

            #region 환자처방 리스트
            grdOrder.AddColumn("recept_no", "접수번호", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdOrder.AddColumn("user_cd", "코드", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("user_nm", "코드명", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("qty", "분량", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdOrder.AddColumn("divide", "몇번에나눠", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdOrder.AddColumn("day", "처방날짜", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdOrder.SetGridHeader();
            grdOrder.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion
        }
        #endregion

        private void GrdReceipt_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdReceipt.ActiveRow != null)
            {
                lblPtntInfo.Text = "환자정보: 접수번호-" + grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString()
                    + "/ 환자명-" + grdReceipt.ActiveRow.Cells["ptnt_nm"].Value.ToString();
            }
        }

        private void GetData()
        {
            #region 접수대기
            DataTable dt = new DataTable();
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"select a.recept_no, b.ptnt_nm, a.clinic_ymd, a.clinic_time 
                                 from h1opdin_test a, hz_mst_ptnt_test b
                                 where a.ptnt_no = b.ptnt_no ;";

            dt = this.FillDataSet(msg).Tables[0];
            grdReceipt.FillData(dt);
            #endregion

            #region 처방
            DataTable dtS = new DataTable();
            DBMessage smsg = new DBMessage();
            smsg.SqlStatement = @"select * from h2opd_doct_ord_test;";

            dtS = this.FillDataSet(smsg).Tables[0];
            grdOrder.FillData(dtS);
            #endregion
        }


    }
}
