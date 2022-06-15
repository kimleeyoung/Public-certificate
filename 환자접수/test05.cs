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
using Mcc.Series.Controls;
using Mcc.Series.Ui.PopUp;
using Infragistics.Win.UltraWinGrid;

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
                   
            this.btnSave.Click += BtnSave_Click;                            //환자정보 저장 및 수정내용 저장
            this.btnDelete.Click += BtnDelete_Click;                        //환자데이터 삭제
            this.btnReceipt.Click += BtnReceipt_Click;                      //환자접수
            this.icnClear.Click += IcnClear_Click;                          //텍스트박스 클리어
            this.grdPtnt.DoubleClick += GrdPtnt_DoubleClick;                //환자정보 수정
            this.txtPtntAddr.KeyPress += TxtAddr_KeyPress;                  //주소입력
            this.txtPtntAddr.SearchButton.Click += SearchButton_Click;      //주소검색               
            this.Text = "환자등록";            
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
                GetPtntData();
                GetOrderData();
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
            grdPtnt.AddColumn("ptnt_no", "환자번호", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.AddColumn("ptnt_nm", "환자이름", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.AddColumn("addr", "환자주소", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);            
            grdPtnt.AddColumn("att_dept", "진료과", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPtnt.SetGridHeader();
            grdPtnt.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;


            grdReceipt.AddColumn("recept_no", "접수번호", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("ptnt_nm", "환자명", 60, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("clinic_ymd", "진료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("clinic_time", "진료시간", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.AddColumn("symp_txt", "증상", 100, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdReceipt.SetGridHeader();
            grdReceipt.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        }
        #endregion

        #region - event-
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (txtPtntNo.Text.Equals("") || txtPtntNm.Text.Equals("") || txtPtntAddr.Text.Equals(""))
            {
                MessageBox.Show("모두 작성 후 저장해주세요.");
                return;
            }

            if (txtPtntNo.Enabled == false)
            {
                if (MessageBox.Show("입력한 데이터로 수정하시겠습니까?", "수정", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    UpdateData();
                    GetPtntData();
                    GetOrderData();
                    Clear();
                }
                else
                {
                    return;
                }
            }
            else
            {
                foreach (UltraGridRow row in grdPtnt.Rows)
                {
                    string ptntNo = row.Cells["ptnt_no"].Value.ToString();

                    if (txtPtntNo.Text == ptntNo)
                    {
                        MessageBox.Show("이미 등록된 동일한 환자입니다.");
                        Clear();
                        return;
                    }
                }
                InsertData();
                GetPtntData();
                Clear();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("환자 데이터를 정말로 삭제하시겠습니까?", "삭제", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                DeleteData();
                GetPtntData();
                GetOrderData();
                Clear();
            }                
        }

        private void BtnReceipt_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("접수 하시겠습니까?","접수", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(grdPtnt.ActiveRow != null)
                {
                    DBMessage msg = new DBMessage();
                    msg.SqlStatement = @"INSERT INTO public.h1opdin_test (recept_no, ptnt_no, clinic_ymd, clinic_time)
                                         VALUES((select ifnull(max(recept_no::integer) + 1, 1) from h1opdin_test),
                                         @ptnt_no, to_char(now(), 'yyyyMMDD'), to_char(now(), 'HH24:MI')); ";
                    msg.AddParameter("ptnt_no", grdPtnt.ActiveRow.Cells["ptnt_no"].Value.ToString());
                    
                    this.ExecuteNonQuery(msg);

                    GetOrderData();
                }
                MessageBox.Show("접수되었습니다");
            }         
        }

        private void IcnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void GrdPtnt_DoubleClick(object sender, EventArgs e)
        {
            if (grdPtnt.Rows.Count > 0)
            {
                txtPtntNo.Text = grdPtnt.ActiveRow.Cells["ptnt_no"].Value.ToString();
                txtPtntNo.Enabled = false;
                txtPtntNm.Text = grdPtnt.ActiveRow.Cells["ptnt_nm"].Value.ToString();
                txtPtntAddr.Text = grdPtnt.ActiveRow.Cells["addr"].Value.ToString();
                cbxPtntDept.Text = grdPtnt.ActiveRow.Cells["att_dept"].Value.ToString();
            }
        }

        private void TxtAddr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.SearchButton_Click(sender, e);
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtPtntAddr.Text)) return;
            
            AddressSearchNew addr = new AddressSearchNew(txtPtntAddr.Text);
            addr.StartPosition = FormStartPosition.CenterParent;

            if (addr.ShowDialog() == DialogResult.OK)
            {
                txtPtntAddr.Text = (addr.StreetDetail + " " + addr.Street).Trim();
            }
        }

        #endregion

        #region - method -        
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

        private void UpdateData()
        {        
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"UPDATE public.hz_mst_ptnt_test
                                 SET ptnt_nm = @ptnt_nm, addr = @addr, att_dept = @att_dept
                                 WHERE ptnt_no = @ptnt_no;";

            msg.AddParameter("ptnt_no", txtPtntNo.Text);
            msg.AddParameter("ptnt_nm", txtPtntNm.Text);
            msg.AddParameter("addr", txtPtntAddr.Text);
            msg.AddParameter("att_dept", cbxPtntDept.SelectedValue.ToString());

            this.ExecuteNonQuery(msg);
        }

        private void InsertData()
        {
            DBMessage msg = new DBMessage();
               
            msg.SqlStatement = $@"INSERT INTO public.hz_mst_ptnt_test
                             (ptnt_no, ptnt_nm, addr, att_dept)
                             VALUES(@ptnt_no, @ptnt_nm, @addr, @att_dept);";

            msg.AddParameter("ptnt_no", txtPtntNo.Text);
            msg.AddParameter("ptnt_nm", txtPtntNm.Text);
            msg.AddParameter("addr", txtPtntAddr.Text);
            msg.AddParameter("att_dept", cbxPtntDept.SelectedValue.ToString());                
                        
            this.ExecuteNonQuery(msg);
        }

        private void DeleteData()
        {
            #region 환자등록 삭제
            DBMessage msg = new DBMessage();
            msg.SqlStatement = $@"DELETE FROM public.hz_mst_ptnt_test WHERE ptnt_no = @ptnt_no";
            msg.AddParameter("ptnt_no", txtPtntNo.Text);

            this.ExecuteNonQuery(msg);
            #endregion

            #region 환자접수 삭제
            DBMessage smsg = new DBMessage();
            smsg.SqlStatement = $@"DELETE from public.h1opdin_test WHERE ptnt_no = @ptnt_no";
            smsg.AddParameter("ptnt_no", grdPtnt.ActiveRow.Cells["ptnt_no"].Value.ToString());

            this.ExecuteNonQuery(smsg);
            #endregion
        }

        private void GetPtntData()
        {
            DataTable dt = new DataTable();
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"select a.ptnt_no, a.ptnt_nm, a.addr, b.dept_nm as att_dept 
                                 from hz_mst_ptnt_test a, hz_mst_dept_test b 
                                 where a.att_dept = b.dept_cd order by a.ptnt_no;";

            dt = this.FillDataSet(msg).Tables[0];             
            grdPtnt.FillData(dt);            
        }

        private void GetOrderData()
        {
            DataTable dt = new DataTable();
            DBMessage msg = new DBMessage();
            msg.SqlStatement = @"select a.recept_no, b.ptnt_nm, a.clinic_ymd, a.clinic_time, a.symp_txt 
                                 from h1opdin_test a, hz_mst_ptnt_test b
                                 where a.ptnt_no = b.ptnt_no   
                                 order by cast(a.recept_no as int);";

            dt = this.FillDataSet(msg).Tables[0];
            grdReceipt.FillData(dt);
        }

        private void Clear()
        {
            txtPtntNo.Clear();
            txtPtntNm.Clear();
            txtPtntAddr.Text = "";
            cbxPtntDept.SelectedIndex = 0;
            txtPtntNo.Enabled = true;            
        }
        #endregion
    }
}