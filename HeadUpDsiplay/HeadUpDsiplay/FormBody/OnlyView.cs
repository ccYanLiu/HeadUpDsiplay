using DevComponents.DotNetBar;
using DevComponents.DotNetBar.Controls;
using HeadUpDsiplay.BLL;
using HeadUpDsiplay.Model;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeadUpDsiplay
{
    public partial class OnlyView : Form
    {
        AutoSizeFormClass asc = new AutoSizeFormClass();
        //日志文件记录
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //定时器
        public Stopwatch stopwatch = new Stopwatch();
        public static bool MesStatus = false;
        private List<TitleHead> titleHeads = new List<TitleHead>();
        public OnlyView()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            stopwatch.Start();

            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Init();
        }

        private void Init()
        {
            LineViewDto root = (LineViewDto)InitService.UseAPI<LineViewDto>(string.Format(ConfigurationManager.AppSettings["Real"], ConfigurationManager.AppSettings["fStation"], ConfigurationManager.AppSettings["line"]),RestSharp.Method.GET);

            if (root != null)
            {
                pictureBox3.Visible = true;
                pictureBox3.Image = Properties.Resources.图片8;
                //显示物料信息
                if (root.Materials != null)
                {
                    for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Materials.Count) / Convert.ToSingle(3)); i++)
                    {
                        if (i >= dataGridView1.Rows.Count)
                        {
                            dataGridView1.Rows.Add();
                        }
                        dataGridView1.Rows[i].Cells[0].Value = root.Materials[i * 3].CHR_NUB_HMK_NAME;
                        dataGridView1.Rows[i].Cells[1].Value = 100;
                        dataGridView1.Rows[i].Cells[2].Value = "";
                        if (i * 3 + 1 < root.Materials.Count)
                        {
                            dataGridView1.Rows[i].Cells[3].Value = root.Materials[i * 3 + 1].CHR_NUB_HMK_NAME;
                            dataGridView1.Rows[i].Cells[4].Value = 100;
                            dataGridView1.Rows[i].Cells[5].Value = "";
                        }
                        if (i * 3 + 2 < root.Materials.Count)
                        {
                            dataGridView1.Rows[i].Cells[6].Value = root.Materials[i * 3 + 2].CHR_NUB_HMK_NAME;
                            dataGridView1.Rows[i].Cells[7].Value = 100;
                            dataGridView1.Rows[i].Cells[8].Value = "";
                             
                        }
                    }
                    for (int i = (int)Math.Ceiling(Convert.ToSingle(root.Materials.Count) / Convert.ToSingle(3)); i < dataGridView1.Rows.Count; i++)
                    {
                        dataGridView1.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    groupBox2.Visible = false;
                    groupBox1.Left = 3;
                    groupBox1.Top = 207;
                }
                if (stopwatch.Elapsed.Seconds < Convert.ToInt32(ConfigurationManager.AppSettings["dt"]))
                {
                    textBox4.Text = ConfigurationManager.AppSettings["fStation"];
                    textBox5.Text = "";
                    var code = root.TileHeads.Where(t => t.Title == "设备PLC通讯:").FirstOrDefault();
                    if (code != null)
                    {
                        if (code.Status == "1")
                            textBox6.Text = "";
                        else
                            textBox6.Text = "N/A";
                    }
                    else
                    {
                        textBox6.Text = "N/A";
                    }
                    code = root.TileHeads.Where(t => t.Title == "物料扫码枪通讯:").FirstOrDefault();
                    if (code != null)
                    {
                        if (code.Status == "1")
                        {
                            textBox7.Text = "";
                            textBox8.Text = "";
                        }
                        else
                        {
                            textBox7.Text = "N/A";
                            textBox8.Text = "N/A";
                        }
                    }
                    else
                    {
                        textBox7.Text = "N/A";
                        textBox8.Text = "N/A";
                    }
                    int cnt = 0;
                    titleHeads = root.TileHeads;
                    foreach (var item in root.TileHeads)
                    {
                        ActiveMatch(++cnt, item.Title, item.Status);
                    }
                    
                    

                    //显示工艺参数信息
                    if (root.Parameters != null)
                    {
                        for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(2)); i++)
                        {
                            if (i >= dataGridView2.Rows.Count)
                            {
                                dataGridView2.Rows.Add();
                                dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                                dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                                dataGridView2.Rows[i].Cells[4].Style.ForeColor = Color.Navy;
                                dataGridView2.Rows[i].Cells[4].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                                //dataGridView2.Rows[i].Cells[7].Style.ForeColor = Color.Navy;
                                //dataGridView2.Rows[i].Cells[7].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            }
                            if (i % 2 == 1)
                            {
                                dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                            }
                            dataGridView2.Rows[i].Cells[0].Value = root.Parameters[i * 2].ParameterName;
                            dataGridView2.Rows[i].Cells[1].Value = 0;
                            dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                           
                            dataGridView2.Rows[i].Cells[2].Value = root.Parameters[i * 2].Spc;
                            if (i * 2 + 1 < root.Parameters.Count)
                            {
                                dataGridView2.Rows[i].Cells[3].Value = root.Parameters[i * 2 + 1].ParameterName;
                                dataGridView2.Rows[i].Cells[4].Value = 0;
                                dataGridView2.Rows[i].Cells[4].Value = 0;
                                dataGridView2.Rows[i].Cells[4].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                                
                                dataGridView2.Rows[i].Cells[5].Value = root.Parameters[i * 2 + 1].Spc;
                            }
                            //if (i * 3 + 2 < root.Parameters.Count)
                            //{
                            //    dataGridView2.Rows[i].Cells[6].Value = root.Parameters[i * 3 + 2].ParameterName;
                            //    dataGridView2.Rows[i].Cells[7].Value = 0;
                            //    dataGridView2.Rows[i].Cells[7].Value = 0;
                            //    dataGridView2.Rows[i].Cells[7].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                                
                            //    dataGridView2.Rows[i].Cells[8].Value = root.Parameters[i * 3 + 2].Spc;
                            //}
                        }

                        for (int i = (int)Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(2)); i < dataGridView2.Rows.Count; i++)
                        {
                            dataGridView2.Rows.RemoveAt(i);
                        }
                    }
                    else
                    {
                        groupBox1.Visible = false;
                    }
                }
            }
            else
            {
                pictureBox3.Visible = true;
                pictureBox3.Image = Properties.Resources.图片5;
                pictureBox6.Image = Properties.Resources.图片5;
                logger.Error("读取API数据为空");
            }
        }

        private void ControlRefresh()
        {
            LineViewRealInfo root = (LineViewRealInfo)InitService.UseAPI<LineViewRealInfo>(string.Format(ConfigurationManager.AppSettings["Real"], ConfigurationManager.AppSettings["fStation"], ConfigurationManager.AppSettings["line"]), RestSharp.Method.POST);
            if (root != null)
            {
                pictureBox3.Visible = true;
                pictureBox3.Image = Properties.Resources.图片8;
                int cnt = 1;

                for (int i = 0; i < root.TitleHeads.Count; i++)
                {
                    if (i < 3)
                    {
                        var temp = titleHeads.Where(t => t.Title == root.TitleHeads[i].Title).FirstOrDefault();
                        if (temp != null)
                        {
                            ActiveMatch(cnt++, root.TitleHeads[i].Title, root.TitleHeads[i].Status);
                        }
                    }
                    else
                    {
                        ActiveMatch(i + 1, root.TitleHeads[i].Title, root.TitleHeads[i].Status);
                    }
                }
                int num = 2;
                if (root.Materials != null)
                {
                    for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Materials.Count) / Convert.ToSingle(3)); i++)
                    {
                        if (i >= dataGridView1.Rows.Count)
                        {
                            dataGridView1.Rows.Add();
                        }
                        dataGridView1.Rows[i].Cells[0].Value = root.Materials[i * 3].MaterialName;
                        dataGridView1.Rows[i].Cells[1].Value = root.Materials[i * 3].Percentage;
                        if (root.Materials[i * 3].WarnStatus == "1")
                        {
                            dataGridView1.Rows[i].Cells[2].Value = "换箱扫码";
                            if (dataGridView1.Rows[i].Cells[2].Style.ForeColor == Color.Red)
                            {
                                dataGridView1.Rows[i].Cells[2].Style.ForeColor = Color.White;
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[2].Style.ForeColor = Color.Red;
                            }
                            dataGridView1.Rows[i].Cells[2].Style.Font = new Font("微软雅黑", 18, FontStyle.Bold);
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[2].Value = "";
                        }

                        if (i * 3 + 1 < root.Materials.Count)
                        {
                            dataGridView1.Rows[i].Cells[3].Value = root.Materials[i * 3 + 1].MaterialName;
                            dataGridView1.Rows[i].Cells[4].Value = root.Materials[i * 3 + 1].Percentage;
                            if (root.Materials[i * 3 + 1].WarnStatus == "1")
                            {
                                dataGridView1.Rows[i].Cells[5].Value = "换箱扫码";
                                if (dataGridView1.Rows[i].Cells[5].Style.ForeColor == Color.Red)
                                {
                                    dataGridView1.Rows[i].Cells[5].Style.ForeColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[i].Cells[5].Style.ForeColor = Color.Red;
                                }
                                dataGridView1.Rows[i].Cells[5].Style.Font = new Font("微软雅黑", 18, FontStyle.Bold);
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[5].Value = "";
                            }
                        }
                        if (i * 3 + 2 < root.Materials.Count)
                        {
                            dataGridView1.Rows[i].Cells[6].Value = root.Materials[i * 3 + 2].MaterialName;
                            dataGridView1.Rows[i].Cells[7].Value = root.Materials[i * 3 + 2].Percentage;
                            if (root.Materials[i * 3 + 2].WarnStatus == "1")
                            {
                                dataGridView1.Rows[i].Cells[8].Value = "换箱扫码";
                                if (dataGridView1.Rows[i].Cells[8].Style.ForeColor == Color.Red)
                                {
                                    dataGridView1.Rows[i].Cells[8].Style.ForeColor = Color.White;
                                }
                                else
                                {
                                    dataGridView1.Rows[i].Cells[8].Style.ForeColor = Color.Red;
                                }
                                dataGridView1.Rows[i].Cells[8].Style.Font = new Font("微软雅黑", 18, FontStyle.Bold);
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[8].Value = "";
                            }

                        }
                    }
                }

                if (root.Parameters.Count > 0)
                {
                    for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(num)); i++)
                    {
                        if (i >= dataGridView2.Rows.Count)
                        {
                            dataGridView2.Rows.Add();
                            dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                            dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            dataGridView2.Rows[i].Cells[4].Style.ForeColor = Color.Navy;
                            dataGridView2.Rows[i].Cells[4].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                        }
                        if (i % 2 == 1)
                        {
                            dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        dataGridView2.Rows[i].Cells[0].Value = root.Parameters[i * 2].ParameterName;
                        dataGridView2.Rows[i].Cells[1].Value = root.Parameters[i * 2].Data;
                        if (root.Parameters[i * 2].Status == "0")
                        {
                            dataGridView2.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                        }
                        else
                        {
                            dataGridView2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                        }
                        dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                        dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                        dataGridView2.Rows[i].Cells[2].Value = root.Parameters[i * 2].Spc;
                        if (i * num + 1 < root.Parameters.Count)
                        {
                            dataGridView2.Rows[i].Cells[3].Value = root.Parameters[i * num + 1].ParameterName;
                            dataGridView2.Rows[i].Cells[4].Value = root.Parameters[i * num + 1].Data;
                            if (root.Parameters[i * num + 1].Status == "0")
                            {
                                dataGridView2.Rows[i].Cells[4].Style.BackColor = Color.LimeGreen;
                            }
                            else
                            {
                                dataGridView2.Rows[i].Cells[4].Style.BackColor = Color.Red;
                            }
                            dataGridView2.Rows[i].Cells[4].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            dataGridView2.Rows[i].Cells[5].Value = root.Parameters[i * 2 + 1].Spc;
                        }
                    }
                }
                else
                {
                    groupBox1.Visible = false;
                }
            }
            else
            {
                if (GlobalData.ServerStatus)
                {
                    pictureBox3.Image = Properties.Resources.图片8;
                }
                else 
                {
                    pictureBox3.Image = Properties.Resources.图片5;
                }
            } 
        }


        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                ControlRefresh();
                if (MesStatus != GlobalData.ServerStatus)
                {
                    if (!MesStatus && GlobalData.ServerStatus)
                    {
                        Init();
                        MesStatus = GlobalData.ServerStatus;
                    }
                    else if (MesStatus && !GlobalData.ServerStatus)
                    {
                        MesStatus = GlobalData.ServerStatus;
                    }
                }
            }
            catch (Exception ex)
            {
                
                logger.Error("读取API数据异常"+ex.Message);
            }

            label2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void ActiveMatch(int num,string text,string value)
        {
            switch (num)
            {
                
                case 1://打印机
                    label8.Text = text;
                    pictureBox4.Visible = true;

                    if (value != "0")
                        pictureBox4.Image = Properties.Resources.图片8;
                    else
                        pictureBox4.Image = Properties.Resources.图片5;
                    break;
                case 2://PLC
                    label9.Text = text;
                    pictureBox5.Visible = true;

                    if (value != "0")
                        pictureBox5.Image = Properties.Resources.图片8;
                    else
                        pictureBox5.Image = Properties.Resources.图片5;
                    break;
                case 3://物料扫码枪
                    label11.Text = text;
                    pictureBox6.Visible = true;

                    if (value!= "0")
                        pictureBox6.Image = Properties.Resources.图片8;
                    else
                        pictureBox6.Image = Properties.Resources.图片5;
                    break;
                case 4://条码
                    textBox6.Text = value;
                    break;
                case 5://当前物料
                    textBox7.Text = value;
                    break;
                case 6://扫码结果
                    textBox8.Text = text;
                    break;
                case 7://扫码结果
                    textBox5.Text = value;
                    break;
                default:
                    break;
            }
        }

        
    
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            //asc.ControlAutoSize(this);
        }
    }
}
