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
using System.Security.Principal;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using System.Diagnostics;
using Infragistics.Win.UltraWinGrid;
using System.Configuration;
using System.Data.SqlClient;

namespace Mcc.Clinic.Common.D._POPUP
{
    enum Status
    {
        OLD,
        NEW
    }

    public partial class H0ComCertificate_new : Mcc.Series.Ui.FormBase
    {
        #region - member variable -
        List<HospInfo> _hospInfo = new List<HospInfo>();
        HospInfo _selHosp = new HospInfo();
        Status _status = Status.OLD;        
        string _startDate = string.Empty;
        string _endDate = string.Empty;
        string _cbxFileNm = string.Empty;
        string _loadFolderNm = string.Empty;
        string _path = string.Empty;
        string _folderPath = string.Empty;
        bool _load = true;
        bool _selectedRow = false;

        string str = string.Empty;
        #endregion

        #region - constructor -
        public H0ComCertificate_new()
        {
            InitializeComponent();

            this.SetBaseButtonHide(new EnumBaseButtonChoose[] { EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF5
                                                              , EnumBaseButtonChoose.btnbaseF6
                                                              , EnumBaseButtonChoose.btnbaseF7
                                                              , EnumBaseButtonChoose.btnbaseF8
                                                              , EnumBaseButtonChoose.btnbaseF10 });

            this.Load += new EventHandler(H0ComCertificate_new_Load);
            this.FormClosing += new FormClosingEventHandler(H0ComCertificate_new_FormClosing);

            this.btnAutoLogin.Click += BtnAutoLogin_Click;   // 인증서보관 저장하기
            this.btnStaticCert.Click += BtnStaticCert_Click; // 인증서보관 인증서 수동선택
            this.btnSave.Click += BtnSave_Click;             // 인증서보관 인증서 저장(서버)
            this.btnDown.Click += BtnDown_Click;             // 인증서다운 인증서 내려받기
            this.btnKICA.Click += BtnKICA_Click;             // 인증서갱신 인증서 갱신(한국정보인증)

            this.cbxHosp.SelectedValueChanged += CbxHosp_SelectedValueChanged; // 콤보박스 요양기관 선택
            this.chkAutoCert.Click += ChkAutoCert_Click;                       // 자동로그인 체크박스 체크
            this.txtLoginPwd.EnabledChanged += TxtLoginPwd_EnabledChanged;     // 비밀번호 입력
            this.icnPath.MouseHover += IcnPath_MouseHover;                     // 아이콘 마우스호버
        }

        #endregion

        #region - form events / SetInitialize -
        private void H0ComCertificate_new_Load(object sender, EventArgs e)
        {
            try
            {
                // To Do..
                this.SetInitialize();
                this.SetGridHeader();                
                
                GetCert();
                SetComboItem();              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void H0ComCertificate_new_FormClosing(object sender, FormClosingEventArgs e)
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

        #region - SetGridHeader / SetInitialize -
        private void SetGridHeader()
        {
            grdPcCert.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("path", "경로", 100, GridColumnStyle.Default, HiddenType.True, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.SetGridHeader();
            grdPcCert.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            grdDbCert1.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.SetGridHeader();
            grdDbCert1.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            grdDbCert2.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.SetGridHeader();
            grdDbCert2.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;

            grdDbCert3.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.SetGridHeader();
            grdDbCert3.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
        }

        private void SetInitialize()
        {
            // To Do..
            lblbasetitle.Text = "공동인증서 설정/조회";
        }
        #endregion

        #region - event -
        private void BtnAutoLogin_Click(object sender, EventArgs e)
        {
            if (chkAutoCert.Checked && string.IsNullOrEmpty(txtLoginPwd.Text))
            {
                MessageBox.Show("자동로그인을 위한 인증서 암호를 입력해 주세요.");
                txtLoginPwd.Focus();
                return;
            }
            else if (this._status == Status.OLD)
            {

                MessageBox.Show("자동로그인을 위해 먼저 인증서를 등록해 주세요.");
            }
            else
            {
                SaveAutoLogin();
            }
        }

        private void BtnStaticCert_Click(object sender, EventArgs e)
        {
            try
            {
                string fullPath = string.Empty;
                FolderBrowserDialog fb = new FolderBrowserDialog();
                DataTable dt = new DataTable();
                dt.Columns.Add("cert_nm");
                dt.Columns.Add("start_date");
                dt.Columns.Add("end_date");
                dt.Columns.Add("path");

                if (fb.ShowDialog() == DialogResult.OK)
                {
                    fullPath = fb.SelectedPath.ToString();
                }

                if (fullPath.Contains("cn="))
                {
                    string[] fileNm = fb.SelectedPath.Split('=');

                    string szSignCertFile = fb.SelectedPath + @"\signCert.der";
                    X509Certificate2 cert = new X509Certificate2(szSignCertFile);

                    _startDate = cert.NotBefore.ToString("yyyy-MM-dd");
                    _endDate = cert.NotAfter.ToString("yyyy-MM-dd");
                    if (grdPcCert.Rows.Count > 0)
                    {
                        dt = grdPcCert.DataSource as DataTable;

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string fileEndDate = grdPcCert.Rows[i].Cells[2].Text;
                            if(_endDate == fileEndDate)
                            {
                                MessageBox.Show("이미 보관된 인증서 입니다. 인증서를 다시 선택해주세요!");
                                return;
                            }
                        }
                    }
                    dt.Rows.Add(fileNm[1].Substring(0,fileNm[1].Length -3), _startDate, _endDate, fullPath);

                    grdPcCert.FillData(dt);

                    if (grdPcCert.Rows.Count > 0) RedText();                    
                }
            }
            catch (Exception ex)
            {
                    
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (_status == Status.NEW)
            {
                if (MessageBox.Show("저장된 인증서가 존재합니다. 선택한 인증서로 변경하시겠습니까", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Clear();
                    SaveData();
                }                            
            }
            else
            {
                SaveData();
            }         
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("이지스를 관리자 권한으로 실행하세요", "공동인증서 관리", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("인증서를 받으시겠습니까", "내려받기", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();

                if (_status == Status.OLD)
                {
                    sMsg.SqlStatement = @"select * from filesystem.certificate order by gb;";
                }
                else
                {
                    sMsg.SqlStatement = $@"select * from filesystem.certificate_hosp where hosp_cd = '{_selHosp.hosp_no}' order by gb";
                }

                DataTable dt;
                dt = this.FillDataSet(sMsg).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    string path1 = string.Empty;                    
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["gb"].ToString().Equals("1"))
                        {
                            path1 = "C:\\Users\\" + Environment.UserName.ToString() + "\\AppData\\LocalLow\\NPKI\\KICA\\USER\\" + dr["file_nm"].ToString();
                            DirectoryInfo dir = new DirectoryInfo(path1);
                            if (dir.Exists)
                            {
                                if (MessageBox.Show("동일한 이름의 인증서가 존재합니다. 덮어쓰시겠습니까?" + global.Enter + "(인증서가 만료되어 갱신하신 경우에도 이 메시지를 보실수 있습니다.)", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes)
                                    return;

                                string backup_path = "C:\\Users\\" + Environment.UserName.ToString() + "\\AppData\\LocalLow\\NPKI\\KICA\\USER\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\\" + dr["file_nm"].ToString();
                                Directory.CreateDirectory(backup_path);

                                FileInfo[] files = dir.GetFiles();
                                foreach (FileInfo file in files)
                                {
                                    if (file.Extension != ".txt")
                                    {
                                        file.CopyTo(backup_path + "\\" + file.Name);
                                    }
                                }
                            }
                            else
                            {
                                dir.Create();
                            }
                        }
                        else
                        {
                            if (dr["file_nm"].ToString().Contains(".txt")) continue;
                            FileStream fs = new FileStream(path1 + "\\" + dr["file_nm"].ToString(), FileMode.Create);                            
                            BinaryWriter w = new BinaryWriter(fs);                                                    
                            byte[] buf = dr["file"] as byte[];
                            w.Write(buf, 0, buf.Length);                                                       
                            w.Close();
                        }
                    }
                    MessageBox.Show("인증서를 내려받았습니다.", "저장완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("저장된 인증서가 없습니다..", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                GetCert();
            }
        }

        private void BtnKICA_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.signgate.com/renew/stepEntrpsCrtfctCnfirm.sg");
        }

        private void CbxHosp_SelectedValueChanged(object sender, EventArgs e)
        {
            Clear();
            if (!setData_new(cbxHosp.Text))
            {
                setData();
            }
        }

        private void ChkAutoCert_Click(object sender, EventArgs e)
        {
            if (grdDbCert1.Rows.Count > 0)
            {
                if (((CheckBox)sender).Checked)
                {
                    this.txtLoginPwd.Enabled = true;
                }
                else
                {
                    this.txtLoginPwd.Enabled = false;
                    txtLoginPwd.BackColor = Color.Gainsboro;
                }                
            }
            else
            {
                MessageBox.Show("자동로그인을 위해 먼저 인증서를 등록해 주세요.");
            }            
        }

        private void TxtLoginPwd_EnabledChanged(object sender, EventArgs e)
        {
            if (!txtLoginPwd.Enabled)
            {
                txtLoginPwd.BackColor = Color.Gainsboro;
            }
            else
            {
                txtLoginPwd.BackColor = Color.White;
            }
        }

        private void IcnPath_MouseHover(object sender, EventArgs e)
        {
            CellsCollection cc = grdPcCert.ActiveRow.Cells;
            string selPath = cc["path"].Value.ToString();
            icnPath.MccToolTip = "이 경로의 인증서 목록을 불러옵니다. " + selPath;
        }        

        #endregion

        #region - method -
        // 서버에 인증서 저장
        private void SaveData()
        {
            if(grdPcCert.Selected.Rows.Count > 0)
            {
                CellsCollection cc = grdPcCert.ActiveRow.Cells;
                //string selectedHospNm = cc["hosp_nm"].Value.ToString();

                string selectedPath = cc["path"].Value.ToString();
                string[] hospNm = selectedPath.Split('=');

                Npgsql.NpgsqlCommand Mycmd = new Npgsql.NpgsqlCommand();
                Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
                this.BeginTrans(sMsg, Mycmd);

                try
                {
                    string[] path = selectedPath.ToString().Split('\\');
                    DirectoryInfo di = new DirectoryInfo(selectedPath);

                    sMsg.SqlStatement = $@"delete from filesystem.certificate_hosp where hosp_cd = @hosp_cd;
                                                INSERT INTO 
                                                    filesystem.certificate_hosp (hosp_cd, gb, file_nm, file, cn, password)
                                                VALUES(@hosp_cd, '1', @file_nm, @file, @cn, @password); ";

                    sMsg.AddParameter("hosp_cd", _selHosp.hosp_no.ToString());
                    sMsg.AddParameter("file_nm", path[path.Length - 1].ToString());
                    sMsg.AddParameter("file", "");

                    if (chkAutoCert.Checked)
                    {
                        //int tempLength = hospNm[1].IndexOf(',');
                        string certNM = hospNm[1].Substring(0, hospNm[1].Length - 3);
                        string pwd = Encrypt(txtLoginPwd.Text, "eghis123");

                        sMsg.AddParameter("cn", certNM);
                        sMsg.AddParameter("password", pwd);
                    }
                    else
                    {
                        sMsg.AddParameter("cn", null);
                        sMsg.AddParameter("password", null);
                    }
                    this.ExecuteNonQuery(sMsg, Mycmd);


                    foreach (FileInfo f in di.GetFiles())
                    {
                        FileStream fw = new FileStream(f.DirectoryName.ToString() + "\\" + f.Name.ToString(), FileMode.Open, FileAccess.Read);

                        byte[] data = new byte[fw.Length];
                        fw.Read(data, 0, (int)fw.Length);

                        sMsg = new Mcc.Series.DataBase.DBMessage();
                        sMsg.SqlStatement = @"INSERT INTO 
                                            filesystem.certificate_hosp
                                        ( hosp_cd,  gb,  file_nm,  file)
                                        VALUES ( @hosp_cd, 2,  @file_nm,  @file);
                                        ;";
                        sMsg.AddParameter("hosp_cd", _selHosp.hosp_no.ToString());
                        sMsg.AddParameter("file_nm", f.Name.ToString());
                        sMsg.AddParameter("file", data);

                        this.ExecuteNonQuery(sMsg, Mycmd);
                        fw.Close();
                    }
                    this.CommitTrans(Mycmd);
                    MessageBox.Show("인증서가 보관되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);

                    _status = Status.NEW;
                    _selectedRow = true;
                }
                catch (Exception ex)
                {
                    this.RollbackTrans(Mycmd);
                    MessageBox.Show("저장 중 오류가 발생되었습니다. \n" + ex.ToString(), "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!setData_new(cbxHosp.Text))
                {
                    setData();
                }
            }
            else
            {
                MessageBox.Show("인증서를 선택 후 등록해주세요");
            }           
        }

        // 자동로그인(암호저장)
        private void SaveAutoLogin()
        {
            string[] fileNM = _cbxFileNm.Split('=');

            if (fileNM.Length <= 1)
            {
                MessageBox.Show("등록된 인증서의 경로가 잘못되었습니다." + global.Enter + " 인증서 폴더 위치를 확인해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Npgsql.NpgsqlCommand Mycmd = new Npgsql.NpgsqlCommand();
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();

            this.BeginTrans(sMsg, Mycmd);

            try
            {
                sMsg.SqlStatement = @"update filesystem.certificate_hosp set cn	= @cn ,	password = @password 
                                    where hosp_cd = @hosp_cd and   gb = '1'";

                sMsg.AddParameter("hosp_cd", _selHosp.hosp_no.ToString());

                if (chkAutoCert.Checked)
                {
                    sMsg.AddParameter("cn", fileNM[1].Substring(0, fileNM[1].Length - 3));
                    sMsg.AddParameter("password", Encrypt(txtLoginPwd.Text, "eghis123"));
                }
                else
                {
                    sMsg.AddParameter("cn", null);
                    sMsg.AddParameter("password", null);
                }
                this.ExecuteNonQuery(sMsg, Mycmd);

                this.CommitTrans(Mycmd);

                MessageBox.Show("자동로그인 설정 변경이 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                this.RollbackTrans(Mycmd);
                MessageBox.Show("저장 중 오류가 발생되었습니다. \n" + e.ToString(), "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!setData_new(cbxHosp.Text))
            {
                setData();
            }
        }

        // setData_new가 아닌 경우, 현재 보여지는 요양기관에 대한 데이터 가져옴
        private void setData()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"select * from filesystem.certificate a
                                         where a.gb = '1'
                                         order by gb";
            DataTable dt;
            dt = this.FillDataSet(sMsg).Tables[0];            
        }

        // 콤보박스의 요양기관들 중 현재 보여지는 것이 아닌 요양기관 선택 시 데이터 가져옴
        private bool setData_new(string hospNm)
        {
            _selHosp = _hospInfo.AsEnumerable().Where(x => x.hosp_nm == hospNm).FirstOrDefault();

            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = $@"select * from filesystem.certificate_hosp a
                                         where a.gb = '1' and a.hosp_cd = '{_selHosp.hosp_no}' 
                                         order by gb";
                        
            DataTable dt;
            dt = this.FillDataSet(sMsg).Tables[0];            

            if (dt.Rows.Count > 0)
            {               
                _cbxFileNm = dt.Rows[0]["file_nm"].ToString();    
                string[] certNm = _cbxFileNm.Split('=');
                
                _status = Status.NEW;

                if (!string.IsNullOrEmpty(dt.Rows[0]["cn"].ToString()))
                {
                    this.chkAutoCert.Checked = true;
                    this.txtLoginPwd.Enabled = true;
                    this.txtLoginPwd.Text = Decrypt(dt.Rows[0]["password"].ToString(), "eghis123");
                }
                else
                {
                    this.chkAutoCert.Checked = false;
                    this.txtLoginPwd.Enabled = false;
                    this.txtLoginPwd.Text = "";
                }

                SetGrid();
                if(grdDbCert3.Rows.Count > 0)
                {
                    lblCertInfo.Text = "[ " + certNm[1].Substring(0, certNm[1].Length - 3) + " ]" +
                    Environment.NewLine + "[ " + _endDate + " ]";
                }                
                return true;
            }
            else
            {
                _status = Status.OLD;
                return false;
            }
        }

        

        // 서버 저장된 요양기관들 콤보박스에 보여주기
        private void SetComboItem()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"select a.hosp_no,a.hosp_nm 
                                        ,max(case when a.main_yn = 'Y' then a.hosp_no else '' end) over() as main_hosp_cd
                                from hz_mst_hosp a
                                order by main_yn desc, a.hosp_no";


            DataTable dt;
            dt = this.FillDataSet(sMsg).Tables[0];
            if (dt.Rows.Count > 0)
            {
                string main = dt.Rows[0]["main_hosp_cd"].ToString();

                foreach (DataRow dr in dt.Rows)
                {
                    cbxHosp.Items.Add(dr["hosp_nm"]);
                    _hospInfo.Add(new HospInfo()
                    {
                        hosp_nm = dr["hosp_nm"].ToString()
                                                 ,
                        hosp_no = dr["hosp_no"].ToString()
                                                 ,
                        hosp_main = dr["main_hosp_cd"].ToString()
                    });
                }

                _selHosp = _hospInfo.AsEnumerable().Where(x => x.hosp_no == main).FirstOrDefault();
                cbxHosp.Text = _selHosp.hosp_nm;
            }
            else
            {
                _hospInfo.Clear();
            }
        }

        // 경로에서 인증서 가져오기
        private void GetCert()
        {
            DataTable dt = new DataTable();
            
            dt.Columns.Add("cert_nm");
            dt.Columns.Add("start_date");
            dt.Columns.Add("end_date");
            dt.Columns.Add("path");

            _path = @"C:\Users\" + Environment.UserName.ToString() + @"\AppData\LocalLow\NPKI\KICA\USER";            
                        
            DirectoryInfo di = new DirectoryInfo(_path);            

            if (di.Exists)
            {
                foreach (DirectoryInfo folder in di.GetDirectories())
                {
                    if (folder.Name.Contains("cn="))
                    {
                        _loadFolderNm = folder.Name;
                        string[] folderNm = _loadFolderNm.Split('=');
                        _folderPath = folder.FullName;
                        CertDate();
                        dt.Rows.Add(folderNm[1].Substring(0,folderNm[1].Length-3), _startDate, _endDate, _folderPath);
                        _load = false;
                    }
                    else
                    {
                        foreach (DirectoryInfo folderInFolder in folder.GetDirectories())
                        {
                            if (folderInFolder.Name.Contains("cn="))
                            {
                                _loadFolderNm = folderInFolder.Name;
                                string[] folderNm = _loadFolderNm.Split('=');
                                _folderPath = folderInFolder.FullName;
                                CertDate();
                                dt.Rows.Add(folderNm[1].Substring(0, folderNm[1].Length - 3), _startDate, _endDate, _folderPath);
                            }                            
                        }
                    }                    
                }                
                grdPcCert.FillData(dt);
                RedText();                
            }            
        }

        // 인증서 시작일자 만료일자 가져오기
        private void CertDate()
        {            
            string szSignCertFile = string.Empty;
            // 처음실행시 인증서 보관
            // 내PC에 보관된 인증서 리스트 가져온다.
            if (_load)
            {
                szSignCertFile = @"C:\Users\" + Environment.UserName.ToString() + @"\AppData\LocalLow\NPKI\KICA\USER\" + _loadFolderNm + @"\signCert.der";
            }
            else if (_selectedRow == true)
            {
                CellsCollection cc = grdPcCert.ActiveRow.Cells;
                szSignCertFile = cc["path"].Value.ToString() + @"\signCert.der";
            }
            else
            {
                // 서버에 저장된 인증서 위치경로를 가져온다
                // 그런데 포맷할경우 인증서가 위치에 없네 !?!?!  에러 ! 
                szSignCertFile = _folderPath + @"\signCert.der";
            }


            X509Certificate2 cert = new X509Certificate2(szSignCertFile);

            _startDate = cert.NotBefore.ToString("yyyy-MM-dd");
            _endDate = cert.NotAfter.ToString("yyyy-MM-dd");
        }

        // 서버에 저장된 인증서 보여주는 그리드 만들기
        private void SetGrid()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("cert_nm");
            dt.Columns.Add("start_date");
            dt.Columns.Add("end_date");

            DirectoryInfo di = new DirectoryInfo(_path);
            if (di.Exists)
            {
                foreach (DirectoryInfo folder in di.GetDirectories())
                {
                    if (folder.Name.Contains(_cbxFileNm))
                    {
                        //CertDate();
                        DbCert();

                        dt.Rows.Add(_cbxFileNm, _startDate, _endDate);

                        grdDbCert1.FillData(dt);
                        grdDbCert2.FillData(dt);
                        grdDbCert3.FillData(dt);

                        RedText();
                    }                    
                }
            }       
        }

        private void DbCert()
        {
            _selHosp = _hospInfo.AsEnumerable().Where(x => x.hosp_nm == cbxHosp.Text).FirstOrDefault();
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = $@"select * from filesystem.certificate_hosp 
                                    where gb = '2' and file_nm = 'signCert.der' and hosp_cd = '{_selHosp.hosp_no}'";
            DataTable dt;
            dt = this.FillDataSet(sMsg).Tables[0];
            
            if (dt.Rows.Count > 0)
            {
                byte[] buf = dt.Rows[0]["file"] as byte[];
                string str = Encoding.Default.GetString(buf);

                var certificate = new X509Certificate2(Convert.FromBase64String(str));
                var exportedPrivate = certificate.PrivateKey.ToXmlString(true);                
            }
        }

        // 만료일자 기준 빨간색 처리
        private void RedText()
        { 
            foreach(UltraGridRow ur in grdDbCert1.Rows)
            {
                string expireDate = ur.Cells["end_date"].Value.ToString();                          
                DateTime endDate = Convert.ToDateTime(expireDate);
                DateTime now = DateTime.Now;
                TimeSpan days = endDate.Subtract(now);
                if (days.TotalDays < 30)
                {
                    ur.Appearance.ForeColor = Color.Red;                    
                    grdDbCert2.Rows[0].Appearance.ForeColor = Color.Red;
                    grdDbCert3.Rows[0].Appearance.ForeColor = Color.Red;
                }                
            }

            foreach (UltraGridRow ur in grdPcCert.Rows)
            {
                string expireDate = ur.Cells["end_date"].Value.ToString();
                DateTime endDate = Convert.ToDateTime(expireDate);
                DateTime now = DateTime.Now;
                TimeSpan days = endDate.Subtract(now);
                if (days.TotalDays < 30)
                {
                    ur.Appearance.ForeColor = Color.Red;
                }
            }
        }

        //초기화
        private void Clear()
        {
            grdDbCert1.RowsClearAll();
            grdDbCert2.RowsClearAll();
            grdDbCert3.RowsClearAll();

            lblCertInfo.Text = "";
            txtLoginPwd.Text = "";
            txtLoginPwd.Enabled = false;
            chkAutoCert.Checked = false;
        }

        //암호화
        private string Encrypt(string p_data, string key)
        {
            byte[] Skey = ASCIIEncoding.ASCII.GetBytes(key);//8자리만 가능

            if (Skey.Length != 8) return string.Empty;

            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();
            rc2.Key = Skey;
            rc2.IV = Skey;
            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateEncryptor(), CryptoStreamMode.Write);
            byte[] data = Encoding.UTF8.GetBytes(p_data.ToCharArray());
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        //복호화
        private static string Decrypt(string pwd, string key)
        {
            byte[] Skey = ASCIIEncoding.ASCII.GetBytes(key);
            DESCryptoServiceProvider rc2 = new DESCryptoServiceProvider();

            rc2.Key = Skey;
            rc2.IV = Skey;

            MemoryStream ms = new MemoryStream();
            CryptoStream cryStream = new CryptoStream(ms, rc2.CreateDecryptor(), CryptoStreamMode.Write);
            byte[] data = Convert.FromBase64String(pwd);
            cryStream.Write(data, 0, data.Length);
            cryStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(ms.GetBuffer());
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        #endregion


        class HospInfo
        {
            public string hosp_no { get; set; }
            public string hosp_nm { get; set; }
            public string hosp_main { get; set; }
        }
    }
}
