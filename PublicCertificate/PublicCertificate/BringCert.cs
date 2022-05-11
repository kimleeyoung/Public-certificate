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
using Infragistics.Win.UltraWinGrid;
using static Mcc.Series.Ui.PopUp.CodeHelp;

namespace PublicCertificate
{
    public partial class BringCert : Mcc.Series.Ui.FormBase
    {
        #region - member variable -
        private DataTable _dt;
        private int _intColCount = 2;
        private string[] _colNames;
        private int[] _colSizes;
        private int _searchType = 0;
        private string[] _usecolumns;
        private string _sSearchTxt = string.Empty;
        private string _strName = string.Empty;
        private bool _multiselect = false;
        private SearchMethod _searchmethod = SearchMethod.KeyPress;


        public DataTable SetDataTable
        {
            get
            {
                return _dt;
            }
            set
            {
                _dt = value;

            }
        }
        public string ColNames
        {
            set
            {
                char[] del = { ',' };
                _colNames = value.Split(del);
            }
        }
        public string ColSizes
        {
            set
            {
                char[] del = { ',' };
                string[] sTmp = value.Split(del);
                _colSizes = new int[sTmp.Length];
                for (int ii = 0; ii < sTmp.Length; ii++)
                {
                    _colSizes[ii] = Convert.ToInt32(sTmp[ii]);
                }
            }
        }
        public int ColCount
        {
            get
            {
                return _intColCount;
            }
            set
            {
                _intColCount = value;
            }
        }
        public string UseColumns
        {
            set
            {
                char[] del = { ',' };
                string[] sTmp = value.Split(del);
                _usecolumns = new string[sTmp.Length];
                for (int ii = 0; ii < sTmp.Length; ii++)
                {
                    _usecolumns[ii] = sTmp[ii].Trim();
                }
            }
        }
        public int searchType
        {
            get
            {
                return _searchType;
            }
            set
            {
                _searchType = value;
            }
        }
        public string ReturnName
        {
            get
            {
                return _strName;
            }
            set
            {
                _strName = value;
            }
        }
        #endregion

        #region - constructor -
        public BringCert()
        {
            InitializeComponent();

            this.SetBaseButtonHide(new EnumBaseButtonChoose[] { EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF7
                                                              , EnumBaseButtonChoose.btnbaseF8
                                                              , EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF10 });

            this.Load += new EventHandler(BringCert_Load);
            this.FormClosing += new FormClosingEventHandler(BringCert_FormClosing);

            grdSavedCert.DoubleClick += GrdSavedCert_DoubleClick;
            mccSearchTextBox1.KeyPress += MccSearchTextBox1_KeyPress;
            mccSearchTextBox1.KeyDown += MccSearchTextBox1_KeyDown;
        }

        private void MccSearchTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData.Equals(Keys.Up))
                {
                    grdSavedCert.Focus();
                }
                else if (e.KeyData.Equals(Keys.Down))
                {
                    if (grdSavedCert.Rows.Count > 0)
                        grdSavedCert.Rows[0].Activate();
                    grdSavedCert.Focus();
                }
                else if (e.KeyData.Equals(Keys.Enter))
                {
                    DataTable dt = this.SetDataTable;

                    if (!string.IsNullOrEmpty(mccSearchTextBox1.Text))
                    {
                        if (mccSearchTextBox1.Text != _sSearchTxt)
                        {
                            _sSearchTxt = mccSearchTextBox1.Text;

                            if (dt.Select(" " + dt.Columns[0].ColumnName.ToString() + " like '%" + mccSearchTextBox1.Text + "%' or " + dt.Columns[1].ColumnName.ToString() + " like'%" + mccSearchTextBox1.Text + "%' ").Count() > 0)
                            {
                                DataTable dt1 = dt.Select(" " + dt.Columns[0].ColumnName.ToString() + " like '%" + mccSearchTextBox1.Text + "%' or " + dt.Columns[1].ColumnName.ToString() + " like'%" + mccSearchTextBox1.Text + "%' ").CopyToDataTable();
                                grdSavedCert.FillData(dt1);
                            }
                            else
                                grdSavedCert.RowsClearAll();
                        }
                        else
                        {
                            if (grdSavedCert.Rows.Count > 0)
                                grdSavedCert.Rows[0].Activate();
                            grdSavedCert.Focus();
                        }
                    }
                    else
                    {
                        grdSavedCert.FillData(dt.Copy());
                        if (grdSavedCert.Rows.Count > 0)
                            grdSavedCert.Rows[0].Activate();
                        grdSavedCert.Focus();
                    }

                }
            }
            catch// (Exception ex)
            {
                //MessageBox.Show(ex.Message.ToString());
            }
        }

        private void MccSearchTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_searchmethod == SearchMethod.EnterKey && e.KeyChar == 13)
            {
                this.Cursor = Cursors.WaitCursor;

                //MessageBox.Show(txtSearch.Text);
                foreach (UltraGridRow row in grdSavedCert.Rows)
                {
                    if (mccSearchTextBox1.Text.Equals(string.Empty))
                    {
                        break;
                    }
                    else if (row.Cells[_searchType].Text.StartsWith(mccSearchTextBox1.Text))
                    {
                        grdSavedCert.ActiveRow = row;
                        break;
                    }
                }

                this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region - form events / SetInitialize -
        private void BringCert_Load(object sender, EventArgs e)
        {
            try
            {
                // To Do..
                this.SetInitialize();
                this.SetGridHeader();

                if (_dt.Rows.Count > 0)
                {
                    this.Show();
                    //this.SetGridHeader();

                    grdSavedCert.FillData(_dt.Copy());

                    mccSearchTextBox1.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BringCert_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {

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
            lblbasetitle.Text = "등록된 인증서";
        }

        private void SetGridHeader()
        {

            for (int i = 0; i < ((_intColCount <= _dt.Columns.Count) ? _intColCount : _dt.Columns.Count); i++)
            {
                if (_colSizes[i].Equals(0))
                    grdSavedCert.AddColumn(_dt.Columns[i].ToString(), _colNames[i], 50, GridColumnStyle.Default, HiddenType.True, ReadOnlyType.True, GridMaskStyle.None);
                else
                    grdSavedCert.AddColumn(_dt.Columns[i].ToString(), _colNames[i], ((_colSizes[i] < 10) ? 50 : _colSizes[i]), GridColumnStyle.Default, HiddenType.False, ReadOnlyType.True, GridMaskStyle.None);

            }

            grdSavedCert.SetGridHeader();


            if (!_multiselect) grdSavedCert.SetReadOnlyGridViewModeStyle(false, FilterUIType.Default, false);
            grdSavedCert.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        }
        #endregion

        private void GrdSavedCert_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                this.SelectItemApply();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void SelectItemApply()
        {
            if ((grdSavedCert.Rows.Count < 1) || (grdSavedCert.ActiveRow == null)) return;

            string a = grdSavedCert.ActiveRow.Cells[3].Text.Trim();
            string b = grdSavedCert.ActiveRow.Cells[2].Text.Trim();
            this.ReturnName = grdSavedCert.ActiveRow.Cells[1].Text.Trim();

            if (a == "만료")
            {
                MessageBox.Show("만료된 인증서 입니다. 인증서를 다시 선택해주세요.", "만료된 인증서", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show(" 만료일자: " + b + "\r" + "인증서를 선택하겠습니까?", "인증서 선택", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                else
                {
                    this.DialogResult = DialogResult.OK;
                }
            }
        }



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
    }
}
