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
using Infragistics.Win.UltraWinGrid;

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
            this.btnAdd.Click += BtnAdd_Click;
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
            grdOrder.AddColumn("recept_no", "접수번호", 40, GridColumnStyle.Default, HiddenType.True, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdOrder.AddColumn("user_cd", "코드", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("user_nm", "코드명", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("qty", "투여", 40, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("divide", "횟수", 40, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.AddColumn("day", "일수", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.False, GridMaskStyle.None);
            grdOrder.SetGridHeader();

            grdOrder.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            //grdOrder.AddRow();
            #endregion
        }
        #endregion

        private void GrdReceipt_DoubleClickRow(object sender, Infragistics.Win.UltraWinGrid.DoubleClickRowEventArgs e)
        {
            if (grdReceipt.ActiveRow != null)
            {
                lblPtntInfo.Text = "환자정보: 접수번호-" + grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString()
                    + " / 환자명-" + grdReceipt.ActiveRow.Cells["ptnt_nm"].Value.ToString();
                
                //DataTable dt = new DataTable();
                //dt.Columns.Add("recept_no");
                //dt.Columns.Add("user_cd");
                //dt.Columns.Add("user_nm");
                //dt.Columns.Add("qty");
                //dt.Columns.Add("divide");
                //dt.Columns.Add("day");

                //dt.Rows.Add(grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString(), null, null, null, null, null);
                //grdOrder.FillData(dt);

                UltraGridRow ugr = grdOrder.AddRow();
                ugr.Cells["recept_no"].Value = grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString();
                ugr.Update();
            }
            else
            {
                MessageBox.Show("증상을 입력해주세요");
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            UltraGridRow row = grdOrder.AddRow();
            row.Cells["recept_no"].Value = grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString();
            row.Update();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!grdReceipt.ActiveRow.Equals(null) && !txtSymp.Text.Equals(""))
            {
                DBMessage msg = new DBMessage();
                msg.SqlStatement = @"UPDATE public.h1opdin_test SET symp_txt=@symp_txt WHERE recept_no=@recept_no;";
                msg.AddParameter("symp_txt", txtSymp.Text);
                msg.AddParameter("recept_no", grdReceipt.ActiveRow.Cells["recept_no"].Value.ToString());
                this.ExecuteNonQuery(msg);
                
                if (grdOrder.Rows.Count > 0)
                {
                    foreach (UltraGridRow row in grdOrder.Rows)
                    {
                        DBMessage smsg = new DBMessage();                        
                        smsg.SqlStatement = @"INSERT INTO public.h2opd_doct_ord_test
                                      (recept_no, user_cd, user_nm, qty, divide, day)
                                      VALUES(@recept_no, @user_cd, @user_nm, @qty, @divide, @day);";

                        smsg.AddParameter("recept_no", row.Cells["recept_no"].Value.ToString());
                        smsg.AddParameter("user_cd", row.Cells["user_cd"].Value.ToString());
                        smsg.AddParameter("user_nm", row.Cells["user_nm"].Value.ToString());
                        smsg.AddParameter("qty", row.Cells["qty"].Value.ToString());
                        smsg.AddParameter("divide", row.Cells["divide"].Value.ToString());
                        smsg.AddParameter("day", row.Cells["day"].Value.ToString());
                        this.ExecuteNonQuery(smsg);
                    }
                    MessageBox.Show("저장이 완료되었습니다.");
                }
                
            }
            else
            {
                MessageBox.Show("증상을 입력해주세요");
            }
        }

        private void GetData()
        {
            DataTable dt = new DataTable();
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"select a.recept_no, b.ptnt_nm, a.clinic_ymd, a.clinic_time 
                                 from h1opdin_test a, hz_mst_ptnt_test b
                                 where a.ptnt_no = b.ptnt_no ;";

            dt = this.FillDataSet(msg).Tables[0];
            grdReceipt.FillData(dt);
        }
    }
}
