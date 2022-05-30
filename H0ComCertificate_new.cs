using System;
using System.Collections.Generic;
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
using Infragistics.Win.UltraWinGrid;
using Mcc.Series.DataBase;

namespace Mcc.Clinic.Common.D._POPUP
{
    public partial class H0ComCertificate_new : Mcc.Series.Ui.FormBase
    {
        #region - member variable -                 
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

            this.btnAutoLogin.Click += BtnAutoLogin_Click;                     // 인증서보관 저장하기
            this.btnStaticCert.Click += BtnStaticCert_Click;                   // 인증서보관 인증서 수동선택
            this.btnSave.Click += BtnSave_Click;                               // 인증서보관 인증서 저장(서버)
            this.btnDown.Click += BtnDown_Click;                               // 인증서다운 인증서 내려받기
            this.btnKICA.Click += BtnKICA_Click;                               // 인증서갱신 인증서 갱신(한국정보인증)

            this.cbxHosp.SelectedValueChanged += CbxHosp_SelectedValueChanged; // 콤보박스 요양기관 선택
            this.chkAutoCert.Click += ChkAutoCert_Click;                       // 자동로그인 체크박스 체크
            this.txtLoginPwd.EnabledChanged += TxtLoginPwd_EnabledChanged;     // 비밀번호 입력
            this.icnPath.MouseMove += IcnPath_MouseMove;                       // 아이콘위에 마우스 움직이기   
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

                SetComboItem();  //콤보박스 아이템 채우기
                GetPcCert();     //내 PC에 보관된 인증서                  
                RedText();       //만료일자 30일이내 인증서 빨간색 처리                             
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
            #region - grdPcCert -
            grdPcCert.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.AddColumn("path", "경로", 100, GridColumnStyle.Default, HiddenType.True, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdPcCert.SetGridHeader();
            grdPcCert.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion

            #region - grdDbCert1 -
            grdDbCert1.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert1.SetGridHeader();
            grdDbCert1.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion

            #region - grdDbCert2 -
            grdDbCert2.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert2.SetGridHeader();
            grdDbCert2.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion

            #region - grdDbCert3 -
            grdDbCert3.AddColumn("cert_nm", "인증서명", 200, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.AddColumn("start_date", "시작일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.AddColumn("end_date", "만료일자", 80, GridColumnStyle.Default, HiddenType.False, ReadOnlyType.NoEdit, GridMaskStyle.None);
            grdDbCert3.SetGridHeader();
            grdDbCert3.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            #endregion

        }

        private void SetInitialize()
        {
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
            else if (grdDbCert1.Rows == null)
            {
                MessageBox.Show("자동로그인을 위해 먼저 인증서를 등록해 주세요.");
            }
            else
            {
                if (MessageBox.Show("정말로 저장하시겠습니까", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    SaveAutoLogin();
            }
        }

        private void BtnStaticCert_Click(object sender, EventArgs e)
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

                string sDate = cert.NotBefore.ToString("yyyy-MM-dd");
                string eDate = cert.NotAfter.ToString("yyyy-MM-dd");

                if (grdPcCert.Rows.Count > 0)
                {
                    dt = grdPcCert.DataSource as DataTable;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string fileEndDate = grdPcCert.Rows[i].Cells[2].Text;
                        string fileName = grdPcCert.Rows[i].Cells[0].Text;

                        if (eDate == fileEndDate && fileNm[1].Substring(0, fileNm[1].Length - 3) == fileName)
                        {
                            MessageBox.Show("이미 보관된 인증서 입니다. 인증서를 다시 선택해주세요!");
                            return;
                        }
                    }

                    dt.Rows.Add(fileNm[1].Substring(0, fileNm[1].Length - 3), sDate, eDate, fullPath);
                    grdPcCert.FillData(dt);
                }
                RedText();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (grdPcCert.ActiveRow != null)
            {
                if (grdDbCert1.Rows.Count > 0)
                {
                    if (MessageBox.Show("저장된 인증서가 존재합니다. 선택한 인증서로 변경하시겠습니까", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        CellsCollection cc = grdPcCert.ActiveRow.Cells;
                        string endDate = cc["end_date"].Value.ToString();
                        DateTime end_date = Convert.ToDateTime(endDate);
                        DateTime now_date = DateTime.Now;

                        if (end_date <= now_date)
                        {
                            MessageBox.Show("만료된 인증서 입니다. 다른 인증서를 선택해주세요");
                            return;
                        }
                        else
                        {
                            SaveData();  // 인증서 서버에 저장
                            Clear();     // 서버에 저장된 인증서 그리드 클리어
                            SetData();   // 서버에 저장된 인증서 그리드 채우기
                        }
                    }
                }
                else
                {
                    SaveData(); // 인증서 서버에 저장
                    SetData();  // 서버에 저장된 인증서 그리드 채우기
                }
                RedText();
            }
            else
            {
                MessageBox.Show("선택된 인증서가 없습니다.");
            }
        }

        private void BtnDown_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("인증서를 받으시겠습니까", "내려받기", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
                sMsg.SqlStatement = $@"select * from filesystem.certificate_hosp where hosp_cd = @hosp_cd order by gb";
                sMsg.AddParameter("hosp_cd", cbxHosp.SelectedValue.ToString());

                DataTable dt = this.FillDataSet(sMsg).Tables[0];

                if (dt.Rows.Count > 0)
                {
                    string path1 = string.Empty;
                    string path2 = string.Empty;

                    foreach (DataRow dr in dt.Rows)
                    {
                        // gb=1인 Row 데이터를 path1,2에 내려받음
                        if (dr["gb"].ToString().Equals("1"))
                        {
                            path1 = "C:\\Users\\" + Environment.UserName.ToString() + "\\AppData\\LocalLow\\NPKI\\KICA\\USER\\" + dr["file_nm"].ToString();
                            path2 = "C:\\Program Files\\NPKI\\KICA\\USER\\" + dr["file_nm"].ToString();

                            // ==================================================== 경로1 ==================================================================== //
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

                            // ==================================================== 경로2 ==================================================================== //
                            dir = new DirectoryInfo(path2);
                            if (dir.Exists)
                            {
                                string backup_path = "C:\\Program Files\\NPKI\\KICA\\USER\\" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "\\" + dr["file_nm"].ToString();
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
                        else  // gb=2인 파일들 path1,2에 내려받음
                        {
                            if (dr["file_nm"].ToString().Contains(".txt")) continue;
                            
                            byte[] buf1 = dr["file"] as byte[];
                            File.WriteAllBytes(path1 + "\\" + dr["file_nm"].ToString(), buf1);

                            byte[] buf2 = dr["file"] as byte[];
                            File.WriteAllBytes(path2 + "\\" + dr["file_nm"].ToString(), buf2);
                        }
                    }
                    MessageBox.Show("인증서를 내려받았습니다.", "저장완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("저장된 인증서가 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                grdPcCert.RowsClearAll();   // 내PC 저장된 인증서 그리드 클리어
                GetPcCert();                // 내PC 저장된 인증서 그리드 채우기
                RedText();                  // 만료 30일이내 인증서 빨간색 표시
            }
        }

        private void BtnKICA_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.signgate.com/renew/stepEntrpsCrtfctCnfirm.sg");
        }

        private void CbxHosp_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbxHosp.SelectedValue == null) return;

            if (((Control)sender).Visible)
            {
                Clear();      // 서버에 저장된 인증서 그리드 클리어
                SetData();    // 서버에 저장된 인증서 + 자동로그인 데이터                 
                RedText();
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

        private void IcnPath_MouseMove(object sender, MouseEventArgs e)
        {
            if (grdPcCert.ActiveCell != null)
            {
                CellsCollection cc = grdPcCert.ActiveRow.Cells;
                string selPath = cc["path"].Value.ToString();
                icnPath.MccToolTip = "이 경로의 인증서 목록을 불러옵니다. " + selPath;
            }
            else
            {
                icnPath.MccToolTip = "어떤 경로에 보관된 인증서인지 보고싶으시면 셀을 클릭한 후 이곳에 마우스를 올려주세요. ";
            }
        }
        #endregion

        #region - method -
        // 요양기관 콤보 채우기
        private void SetComboItem()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"select a.hosp_no, a.hosp_nm, case when a.main_yn = 'Y' then a.hosp_no else '' end as main_hosp_cd
                                  from hz_mst_hosp a order by main_hosp_cd desc;";

            DataTable dt = this.FillDataSet(sMsg).Tables[0];

            if (dt.Rows.Count > 0)                                                    //요양기관 데이터가 있을 경우
            {
                cbxHosp.FillData(dt, "hosp_nm", "hosp_no", AddingItemMode.None);      //일단 요양기관 콤보 채우기
                
                if (string.IsNullOrEmpty(dt.Rows[0]["main_hosp_cd"].ToString()))      //대표 요양기관기호로 기본값 불러오기
                {
                    cbxHosp.SelectedIndex = 0;
                }                
            }
            else
            {
                MessageBox.Show("요양기관에 대한 정보가 없습니다. 병원정보를 등록해주세요");
            }
        }

        //서버에 저장된 (인증서 + 자동 로그인) 데이터 가져오기
        private void SetData()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = $@"select * from filesystem.certificate_hosp where hosp_cd = @hosp_no order by gb";
            sMsg.AddParameter("hosp_no", cbxHosp.SelectedValue.ToString());

            DataTable dt = this.FillDataSet(sMsg).Tables[0];
            
            if (dt.Rows.Count > 0)
            {
                string[] cbxCertNm = new string[10];
                string sDate = string.Empty;
                string eDate = string.Empty;

                DataTable dtT = new DataTable();
                dtT.Columns.Add("cert_nm");
                dtT.Columns.Add("start_date");
                dtT.Columns.Add("end_date");

                //서버에 저장된 인증서 데이터 가져오기
                foreach (DataRow dr in dt.Rows)
                {
                    if (dr["gb"].ToString().Equals("1"))
                    {
                        cbxCertNm = dt.Rows[0]["file_nm"].ToString().Split('=');
                    }
                    else
                    {
                        if (dr["file_nm"].ToString().Equals("signCert.der"))
                        {                            
                            byte[] buf = dr["file"] as byte[];
                            X509Certificate2 cert = new X509Certificate2(buf);

                            sDate = cert.NotBefore.ToString("yyyy-MM-dd");
                            eDate = cert.NotAfter.ToString("yyyy-MM-dd");
                        }
                    }
                    
                }            
                dtT.Rows.Add(cbxCertNm[1].Substring(0, cbxCertNm[1].Length - 3), sDate, eDate);

                grdDbCert1.FillData(dtT);
                grdDbCert2.FillData(dtT);
                grdDbCert3.FillData(dtT);
                
                lblCertInfo.Text = "[ " + cbxCertNm[1].Substring(0, cbxCertNm[1].Length - 3) + " ]" +
                    Environment.NewLine + "[ " + dtT.Rows[0]["end_date"].ToString() + " ]";
            
                // 서버에 저장된 자동로그인 데이터 가져오기
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
            }
        }
        
        // 서버에 인증서 저장
        private void SaveData()
        {
            CellsCollection cc = grdPcCert.ActiveRow.Cells;

            string selectedPath = cc["path"].Value.ToString();
            string[] hospNm = cc["path"].Value.ToString().Split('=');
            
            //Folder 정보 Db 저장
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            string[] path = selectedPath.ToString().Split('\\');

            sMsg.SqlStatement = $@"delete from filesystem.certificate_hosp where hosp_cd = @hosp_cd;
                                   INSERT INTO filesystem.certificate_hosp (hosp_cd, gb, file_nm, file, cn, password)
                                   VALUES(@hosp_cd, '1', @file_nm, @file, @cn, @password);";

            sMsg.AddParameter("hosp_cd", cbxHosp.SelectedValue.ToString());
            sMsg.AddParameter("file_nm", path[path.Length - 1].ToString());
            sMsg.AddParameter("file", "");

            //패스워드 Db 저장
            if (chkAutoCert.Checked)
            {
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

            this.ExecuteNonQuery(sMsg);

            //File 정보 Db 저장
            DirectoryInfo di = new DirectoryInfo(selectedPath);
            foreach (FileInfo f in di.GetFiles())
            {
                FileStream fw = new FileStream(f.DirectoryName.ToString() + "\\" + f.Name.ToString(), FileMode.Open, FileAccess.Read);

                byte[] data = new byte[fw.Length];
                fw.Read(data, 0, (int)fw.Length);

                sMsg = new Mcc.Series.DataBase.DBMessage();
                sMsg.SqlStatement = @"INSERT INTO filesystem.certificate_hosp( hosp_cd,  gb,  file_nm,  file)
                                      VALUES(@hosp_cd, 2,  @file_nm,  @file);";

                sMsg.AddParameter("hosp_cd", cbxHosp.SelectedValue.ToString());
                sMsg.AddParameter("file_nm", f.Name.ToString());
                sMsg.AddParameter("file", data);
                
                this.ExecuteNonQuery(sMsg);

                fw.Close();
            }

            MessageBox.Show("서버에 인증서가 보관되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);           
        }

        // 자동로그인(암호저장)
        private void SaveAutoLogin()
        {
            CellsCollection cc = grdDbCert1.ActiveRow.Cells;
            string fileNM = cc["cert_nm"].Value.ToString();

            if (fileNM.Length <= 1)
            {
                MessageBox.Show("등록된 인증서의 경로가 잘못되었습니다." + global.Enter + " 인증서 폴더 위치를 확인해주세요", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"update filesystem.certificate_hosp set cn	= @cn ,	password = @password 
                                  where hosp_cd = @hosp_cd and gb = '1'";

            sMsg.AddParameter("hosp_cd", cbxHosp.SelectedValue.ToString());

             if (chkAutoCert.Checked)
            {
                sMsg.AddParameter("cn", fileNM);
                sMsg.AddParameter("password", Encrypt(txtLoginPwd.Text, "eghis123"));
            }
            else
            {
                txtLoginPwd.Text = "";
                sMsg.AddParameter("cn", null);
                sMsg.AddParameter("password", null);
            }
            this.ExecuteNonQuery(sMsg);
            
            MessageBox.Show("자동로그인 설정 변경이 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);             
        }

        // 내PC 경로에서 인증서 가져오기
        private void GetPcCert()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("cert_nm");
            dt.Columns.Add("start_date");
            dt.Columns.Add("end_date");
            dt.Columns.Add("path");

            string loadFolderNm = string.Empty;
            string folderPath = string.Empty;
            string sDate = string.Empty;
            string eDate = string.Empty;

            string path1 = @"C:\Users\" + Environment.UserName.ToString() + @"\AppData\LocalLow\NPKI\KICA\USER";
            string path2 = @"C:\Program Files\NPKI\KICA\USER";
            
            DirectoryInfo di1 = new DirectoryInfo(path1);            
            DirectoryInfo di2 = new DirectoryInfo(path2);            

            //경로1에서 인증서 가져옴
            if (di1.Exists)
            {
                foreach (DirectoryInfo folder in di1.GetDirectories())
                {
                    if (folder.Name.Contains("cn="))
                    {
                        loadFolderNm = folder.Name;
                        string[] folderNm = loadFolderNm.Split('=');
                        folderPath = folder.FullName;
                        CertDate(folderPath, ref sDate,ref eDate);
                        dt.Rows.Add(folderNm[1].Substring(0, folderNm[1].Length - 3), sDate, eDate, folderPath);
                    }
                }
            }

            //경로2에서 인증서 가져옴
            if (di2.Exists)
            {
                foreach (DirectoryInfo folder in di2.GetDirectories())
                {
                    if (folder.Name.Contains("cn="))
                    {
                        loadFolderNm = folder.Name;
                        string[] folderNm = loadFolderNm.Split('=');
                        folderPath = folder.FullName;
                        CertDate(folderPath, ref sDate, ref eDate);
                        dt.Rows.Add(folderNm[1].Substring(0, folderNm[1].Length - 3), sDate, eDate, folderPath);
                    }
                }
            }

            //경로1,경로2 인증서 중복제거
            for(int i=0; i<dt.Rows.Count; i++)
            {
                string certNm1 = dt.Rows[i]["cert_nm"].ToString();
                for(int j=i+1; j<dt.Rows.Count; j++)                            //인증서 이름 비교
                {
                    string certNm2 = dt.Rows[j]["cert_nm"].ToString();

                    if(certNm1 == certNm2)                                      //인증서 이름이 같으면
                    {                        
                        string endDate1 = dt.Rows[i]["end_date"].ToString();
                        string endDate2 = dt.Rows[j]["end_date"].ToString();

                        if(endDate1 == endDate2)                                //인증서 만료 날짜 같으면     
                        {
                            dt.Rows.Remove(dt.Rows[j]);                         //중복된 두번째 인증서 삭제
                            j = j - 1;
                        }
                    }
                }
            }
            
            grdPcCert.FillData(dt);            
        }
        
        // 내PC 경로에 있는 인증서 시작일자 만료일자 가져오기
        private void CertDate(string fpath, ref string sDate, ref string eDate)
        {            
            string szSignCertFile = string.Empty;
                     
            szSignCertFile = fpath + @"\signCert.der";

            X509Certificate2 cert = new X509Certificate2(szSignCertFile);

            sDate = cert.NotBefore.ToString("yyyy-MM-dd");
            eDate = cert.NotAfter.ToString("yyyy-MM-dd");
        }    

        // 만료일자 기준 빨간색 처리
        private void RedText() 
        {
            if (grdDbCert1.Rows.Count > 0)
            {
                string expireDate = grdDbCert1.Rows[0].Cells["end_date"].Value.ToString();
                DateTime endDate = Convert.ToDateTime(expireDate);
                DateTime now = DateTime.Now;
                TimeSpan days = endDate.Subtract(now);
                if (days.TotalDays < 30)
                {
                    grdDbCert1.Rows[0].Appearance.ForeColor = Color.Red;
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
                else
                {
                    ur.Appearance.ForeColor = Color.Black;
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
        
        #endregion
    }
}
