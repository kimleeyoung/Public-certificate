#region 수정내역
/*
 * -----------------------------------------------------------------------------------------------
 * 2018.12.07 조윤성 : 인증서 선택화면 경로 추가 [INDEX : CYS20181207]
 * 2019.07.05 조윤성 : 인증서 자동로그인만 변경가능하도록 수정 [INDEX : CYS20190705]
 * 2019.07.16 조윤성 : 인증서 자동로그인설정 화면 UI 변경하면서 버튼 추가[INDEX : CYS20190716]
 * 2019.10.02 조윤성 : 백업파일 생성시 txt 파일은 제외시킨다 [INDEX : CYS20191002]
 * -----------------------------------------------------------------------------------------------
 */
#endregion


using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Mcc.Series.Ui.Type;
using Mcc.Series.Common.Enum;
using System.IO;
using System.Security.Cryptography;
using System.Security.Principal;

namespace Mcc.Clinic.Common
{
    enum Status
    {
        OLD,
        NEW
    }

    public partial class H0ComCertificate : Mcc.Series.Ui.FormBase
    {
        #region - member variable -
        List<HospInfo> _hospInfo = new List<HospInfo>();
        HospInfo _selHosp = new HospInfo();
        Status _status = Status.OLD;
        #endregion

        #region - constructor -
        public H0ComCertificate()
        {
            InitializeComponent();

            this.SetBaseButtonHide(new EnumBaseButtonChoose[] { EnumBaseButtonChoose.btnbaseF9
                                                              , EnumBaseButtonChoose.btnbaseF5
                                                              , EnumBaseButtonChoose.btnbaseF6
                                                              , EnumBaseButtonChoose.btnbaseF7
                                                              , EnumBaseButtonChoose.btnbaseF8
                                                              , EnumBaseButtonChoose.btnbaseF10 });
            this.Load += new EventHandler(H0ComCertificate_Load);
            this.FormClosing += new FormClosingEventHandler(H0ComCertificate_FormClosing);
            this.chkAutoCert.Click += ChkAutoCert_Click;
            this.btnDown.Click += new EventHandler(btnDown_Click);
            this.btnSave.Click += BtnSave_Click;
            this.btnStaticCert.Click += BtnStaticCert_Click;
            this.btnAutoCert.Click += BtnAutoCert_Click;
            this.txtCertPwd.Enabled = false;
            this.txtCertPwd.EnabledChanged += TxtCertPwd_EnabledChanged;
            this.cbxHosp.SelectedValueChanged += CbxHosp_SelectedValueChanged;
            this.btnAutoLogin.Click += BtnAutoLogin_Click;
        }
        
        #endregion

        #region - form events / SetInitialize -
        private void H0ComCertificate_Load(object sender, EventArgs e)
        {
            try
            {
                // To Do..
                this.SetInitialize();
                this.SetGridHeader();
                SetComboItem();
                //setData();
                

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "에러", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void H0ComCertificate_FormClosing(object sender, FormClosingEventArgs e)
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
            lblbasetitle.Text = "공동인증서 설정/조회";
            this.Text = "공동인증서 설정/조회";
        }

        private void SetGridHeader()
        {
            // To Do..	
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

        #region - event -
        //인증서 내려받기
        void btnDown_Click(object sender, EventArgs e)
        {
            if (!IsAdministrator())
            {
                MessageBox.Show("이지스를 관리자 권한으로 실행하세요.", "공동인증서 관리", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            

            if (MessageBox.Show("인증서를 받으시겠습니까?", "내려받기", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
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
                    string path2 = string.Empty;
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["gb"].ToString().Equals("1"))
                        {

                            path1 = "C:\\Users\\" + Environment.UserName.ToString() + "\\AppData\\LocalLow\\NPKI\\KICA\\USER\\" + dr["file_nm"].ToString();
                            path2 = "C:\\Program Files\\NPKI\\KICA\\USER\\" + dr["file_nm"].ToString();
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
                        else
                        {
                            if (dr["file_nm"].ToString().Contains(".txt")) continue;
                            FileStream fs = new FileStream(path1 + "\\" + dr["file_nm"].ToString(), FileMode.Create);
                            BinaryWriter w = new BinaryWriter(fs);
                            byte[] buf = dr["file"] as byte[];
                            w.Write(buf, 0, buf.Length);
                            w.Close();

                            fs = new FileStream(path2 + "\\" + dr["file_nm"].ToString(), FileMode.Create);
                            w = new BinaryWriter(fs);
                            buf = dr["file"] as byte[];
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
            }

        }

        //인증서 암호 활성화
        private void TxtCertPwd_EnabledChanged(object sender, EventArgs e)
        {
            if (!txtCertPwd.Enabled)
            {
                txtCertPwd.BackColor = Color.Gainsboro;
            }
            else
            {
                txtCertPwd.BackColor = Color.White;
            }
        }

        private void ChkAutoCert_Click(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                this.txtCertPwd.Enabled = true;
            }
            else
            {
                this.txtCertPwd.Enabled = false;
                txtCertPwd.BackColor = Color.Gainsboro;
            }
        }

        //인증서 선택
        private void BtnAutoCert_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            Mcc.Series.Ui.PopUp.CodeHelp help = new Mcc.Series.Ui.PopUp.CodeHelp();
            dt.Columns.Add("filename");
            dt.Columns.Add("path");
            dt.Columns.Add("date");
           
            DirectoryInfo pfDi = new DirectoryInfo(@"C:\Program Files\NPKI\KICA\USER");
            DirectoryInfo userDi = new DirectoryInfo(@"C:\Users\"+ Environment.UserName.ToString() + @"\AppData\LocalLow\NPKI\KICA\USER");
            if (pfDi.Exists)
            {
                String[] directories = System.IO.Directory.GetDirectories(@"C:\Program Files\NPKI\KICA\USER");
                for (int i = 0; i < directories.Length; i++)
                {
                    FileInfo file = new FileInfo(directories[i]);
                    if (file.Name.Contains("cn="))
                    {
                        string[] fileNM = file.Name.Split('=');
                        int a = fileNM[1].IndexOf(',');

                        dt.Rows.Add(fileNM[1].Substring(0, fileNM[1].Length - 3), file.FullName, file.CreationTime);
                    }
                }
            }
            //CYS20181207---------------------------------------------------------------------------------------------------S
            if (userDi.Exists)
            {
                String[] directories = System.IO.Directory.GetDirectories(@"C:\Users\"+Environment.UserName.ToString()+@"\AppData\LocalLow\NPKI\KICA\USER");
                for (int i = 0; i < directories.Length; i++)
                {
                    FileInfo file = new FileInfo(directories[i]);
                    if (file.Name.Contains("cn="))
                    {
                        string[] fileNM = file.Name.Split('=');
                        int a = fileNM[1].IndexOf(',');

                        dt.Rows.Add(fileNM[1].Substring(0, fileNM[1].Length - 3), file.FullName, file.CreationTime);
                    }
                }
            }
            //CYS20181207---------------------------------------------------------------------------------------------------E

            help.SetDataTatle = dt;
            help.Text = "공동인증서 선택";
            help.Width = 550;
            help.Height = 350;
            help.searchType = 0;
            help.ColCount = 3;
            help.Load += delegate
            {
                help.grdList.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            };
            help.ColNames = "이름,경로,수정일자";
            help.ColSizes = "150,200,200";

            help.UseColumns = "filename,path,date";
            if (help.ShowDialog() == DialogResult.OK)
            {
                if (help.ReturnName != txtCertificate.Text)
                {
                    txtCertPwd.Text = "";
                    chkAutoCert.Checked = false;
                    txtCertificate.Text = help.ReturnName;
                }
            }

        }

        //인증서 수동 선택
        private void BtnStaticCert_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog fb = new FolderBrowserDialog();
                if (txtCertificate.Text != "")
                {
                    fb.SelectedPath = txtCertificate.Text;
                }

                if (fb.ShowDialog() == DialogResult.OK)
                {
                    string fullPath = string.Empty;
                    fullPath = fb.SelectedPath.ToString();
                    this.txtCertificate.Text = fullPath;
                }
            }
            catch
            {
            }
        }

        //인증서 저장
        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        //-------------------------------------------------------------------------------------------------------CYS20190716S
        //인증서 자동로그인 저장
        private void BtnAutoLogin_Click(object sender, EventArgs e)
        {
            if (chkAutoCert.Checked && string.IsNullOrEmpty(txtCertPwd.Text))
            {
                MessageBox.Show("자동로그인을 위한 인증서 암호를 입력 해 주세요.");
                txtCertPwd.Focus();
                return;
            }
            else if(this._status == Status.OLD)
            {

                MessageBox.Show("자동로그인을 위해 먼저 인증서를 등록해 주세요.");
            }
            else
            {
                SaveAutoLogin();
            }
        }
        //------------------------------------------------------------------------------------------------------CYS20190716E

        private void CbxHosp_SelectedValueChanged(object sender, EventArgs e)
        {
            Clear();
            if (!setData_new(cbxHosp.Text))
            {
                setData();
            }
        }
        #endregion
        
        #region - data - 
        private void setData()
        {
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();
            sMsg.SqlStatement = @"select * from filesystem.certificate a
                                         where a.gb = '1'
                                         order by gb";
            DataTable dt;
            dt = this.FillDataSet(sMsg).Tables[0];
            if (dt.Rows.Count > 0)
            {
                lblCertificate.Text = "- " + dt.Rows[0]["file_nm"].ToString();
            }
        }

        //공인인증서 자동로그인을 위해 새로운 테이블생성 하여 데이터 조회 하는 부분 추가 CYS20181114
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
                lblCertificate.Text = "- " + dt.Rows[0]["file_nm"].ToString();
                _status = Status.NEW;

                if(!string.IsNullOrEmpty(dt.Rows[0]["cn"].ToString()))
                {
                    this.chkAutoCert.Checked = true;
                    this.txtCertPwd.Enabled = true;
                    this.txtCertPwd.Text = Decrypt(dt.Rows[0]["password"].ToString(), "eghis123");
                }
                else
                {
                    this.chkAutoCert.Checked = false;
                    this.txtCertPwd.Enabled = false;
                    this.txtCertPwd.Text = "";

                }
                return true;
            }
            else
            {
                _status = Status.OLD;
                return false;
            }
        }
        
        //요양기관번호로 요양기관명 선택할 수 있도록 요양기관명 입력 CYS20181114
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

        private void SaveData()
        {
            string[] fileNM = txtCertificate.Text.Split('=');
            
            #region 오류검증
            if (!txtCertificate.Text.ToString().Trim().ToUpper().Contains("=")) 
            {
                this.txtCertificate.Text = "";
                MessageBox.Show("올바른 인증서가 아닙니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (chkAutoCert.Checked && string.IsNullOrEmpty(txtCertPwd.Text))
            {
                MessageBox.Show("자동로그인을 위한 인증서 암호를 입력 해 주세요.");
                txtCertPwd.Focus();
                return;
            }
            if(fileNM.Length < 8 && fileNM.Length > 6)
            {
                MessageBox.Show("잘못된 인증서 경로 입니다.");
                return;
            }

            #endregion
            
            Npgsql.NpgsqlCommand Mycmd = new Npgsql.NpgsqlCommand();
            Mcc.Series.DataBase.DBMessage sMsg = new Mcc.Series.DataBase.DBMessage();

            this.BeginTrans(sMsg, Mycmd);
            
            try
            {
                string[] path = txtCertificate.Text.ToString().Split('\\');
                DirectoryInfo di = new DirectoryInfo(txtCertificate.Text);
               
                this.txtCertificate.Text = txtCertificate.Text;
                sMsg.SqlStatement = $@"delete from filesystem.certificate_hosp where hosp_cd = @hosp_cd;
                                                INSERT INTO 
                                                    filesystem.certificate_hosp (hosp_cd, gb, file_nm, file, cn, password)
                                                VALUES(@hosp_cd, '1', @file_nm, @file, @cn, @password); ";


                sMsg.AddParameter("hosp_cd", _selHosp.hosp_no.ToString());
                sMsg.AddParameter("file_nm", path[path.Length - 1].ToString());
                sMsg.AddParameter("file", "");
                if (chkAutoCert.Checked)
                {
                    int tempLength = fileNM[1].IndexOf(',');
                    string certNM = fileNM[1].Substring(0, fileNM[1].Length - 3);
                    string pwd = Encrypt(txtCertPwd.Text, "eghis123");

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
                    //byte[] data;

                    FileStream fw = new FileStream(f.DirectoryName.ToString() + "\\" + f.Name.ToString(), FileMode.Open, FileAccess.Read); ;

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
                MessageBox.Show("인증서가 보관되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _status = Status.NEW;
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

        //-----------------------------------------------------------------------------------CYS20190705 S
        private void SaveAutoLogin()
        {
            string[] fileNM = lblCertificate.Text.Split('=');


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
                    sMsg.AddParameter("password", Encrypt(txtCertPwd.Text, "eghis123"));
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
        //-----------------------------------------------------------------------------------CYS20190705 E
        
        //비밀번호 저장시 암호화 CYS20181114
        public string Encrypt(string p_data, string key)
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

        //비밀번호 복호화 CYS20181120
        public static string Decrypt(string pwd, string key)
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
         
        private void Clear()
        {
            txtCertificate.Text = "";
            txtCertPwd.Text = "";
            txtCertPwd.Enabled = false;
            chkAutoCert.Checked = false;
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if(null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }
        #endregion
        
    }

    /// <summary>
    /// 공인인증서 병원 정보
    /// </summary>
    class HospInfo
    {
        public string hosp_no { get; set; }
        public string hosp_nm { get; set; }
        public string hosp_main { get; set; }
    }
}