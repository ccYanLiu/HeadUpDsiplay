using HeadUpDsiplay.Model;
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
    public partial class ShareView : Form
    {
        AutoSizeFormClass asc = new AutoSizeFormClass();
   
        //日志文件记录
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        //定时器
        public Stopwatch fstopwatch = new Stopwatch();
        public Stopwatch sstopwatch = new Stopwatch();
        public static bool MesStatus = false;
        private List<TitleHead> titleHeads = new List<TitleHead>();

        public ShareView()
        {
            InitializeComponent();
        }

        private void ShareView_Load(object sender, EventArgs e)
        {

            timer1.Enabled = true;
            timer2.Enabled = true;
            fstopwatch.Start();
            sstopwatch.Start();

            dataGridView2.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView3.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView3.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView4.ColumnHeadersDefaultCellStyle.Font = new Font("微软雅黑", 18, FontStyle.Bold);
            dataGridView4.ColumnHeadersDefaultCellStyle.BackColor = Color.Navy;
            dataGridView4.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            Init(1);
            Init(2);
        }
        private void Init(int serial)
        {
             
            LineViewDto root = (LineViewDto)InitService.UseAPI<LineViewDto>(string.Format(ConfigurationManager.AppSettings["Real"], serial == 1 ? ConfigurationManager.AppSettings["fStation"] : ConfigurationManager.AppSettings["sStation"], ConfigurationManager.AppSettings["line"]), RestSharp.Method.GET);

            if (root != null)
            {
                if (serial == 1)
                {
                    pictureBox3.Visible = true;
                    pictureBox3.Image = Properties.Resources.图片8;
                }
                else
                {
                    pictureBox10.Visible = true;
                    pictureBox10.Image = Properties.Resources.图片8;
                }
                //显示物料信息
                if (root.Materials != null)
                {
                    for (int i = 0; i < root.Materials.Count; i++)
                    {
                        if (serial == 1)
                        {
                            if (i >= dataGridView1.Rows.Count)
                            {
                                dataGridView1.Rows.Add();
                            }


                            dataGridView1.Rows[i].Cells[0].Value = root.Materials[i].CHR_NUB_HMK_NAME; ;
                            dataGridView1.Rows[i].Cells[1].Value = "";
                             
                        }
                        else
                        {
                            if (i >= dataGridView4.Rows.Count)
                            {
                                dataGridView4.Rows.Add();
                            }
                            dataGridView4.Rows[i].Cells[0].Value = root.Materials[i].CHR_NUB_HMK_NAME;
                            dataGridView4.Rows[i].Cells[1].Value = "";
                        }
                    }
                }
                else
                {
                    groupBox2.Visible = false;
                    groupBox1.Left = 3;
                    groupBox1.Top = 207;
                }
                if (serial == 1)
                {
                    textBox4.Text = serial == 1 ? ConfigurationManager.AppSettings["fStation"] : ConfigurationManager.AppSettings["sStation"];
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
                }
                else
                {
                    textBox12.Text = serial == 1 ? ConfigurationManager.AppSettings["fStation"] : ConfigurationManager.AppSettings["sStation"];
                    textBox11.Text = "";
                    var code = root.TileHeads.Where(t => t.Title == "设备PLC通讯:").FirstOrDefault();
                    if (code != null)
                    {
                        if (code.Status == "1")
                            textBox10.Text = "";
                        else
                            textBox10.Text = "N/A";
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
                }
                
                int cnt = 0;
                titleHeads = root.TileHeads;
                foreach (var item in root.TileHeads)
                {
                    ActiveMatch(++cnt, item.Title, item.Status,serial);
                }



                //显示工艺参数信息
                if (root.Parameters != null)
                {
                    for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(3)); i++)
                    {
                        if (i >= dataGridView2.Rows.Count)
                        {
                            dataGridView2.Rows.Add();
                            dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                            dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                        }
                        if (i % 2 == 1)
                        {
                            dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                        }
                        dataGridView2.Rows[i].Cells[0].Value = root.Parameters[i].ParameterName;
                        dataGridView2.Rows[i].Cells[1].Value = 0;
                        dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                        dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                         
                        dataGridView2.Rows[i].Cells[2].Value = root.Parameters[i].Spc;
                    }

                    for (int i = (int)Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(3)); i < dataGridView2.Rows.Count; i++)
                    {
                        dataGridView2.Rows.RemoveAt(i);
                    }
                }
                else
                {
                    groupBox1.Visible = false;
                }
            }
            else
            {
                if (serial == 1)
                {
                    pictureBox3.Image = Properties.Resources.图片5;
                }
                else
                    pictureBox10.Image = Properties.Resources.图片5;

                logger.Error("读取API数据为空");
            }
        }

        private void ControlRefresh(int serial)
        {
            LineViewRealInfo root = (LineViewRealInfo)InitService.UseAPI<LineViewRealInfo>(string.Format(ConfigurationManager.AppSettings["Real"], serial == 1 ?ConfigurationManager.AppSettings["fStation"]: ConfigurationManager.AppSettings["sStation"], ConfigurationManager.AppSettings["line"]), RestSharp.Method.POST);
            if (root != null)
            {
                if (serial == 1)
                {
                    pictureBox3.Visible = true;
                    pictureBox3.Image = Properties.Resources.图片8;
                    int num = 2;
                    for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Materials.Count) / Convert.ToSingle(3)); i++)
                    {
                        if (i >= dataGridView1.Rows.Count)
                        {
                            dataGridView1.Rows.Add();
                        }
                        dataGridView1.Rows[i].Cells[0].Value = root.Materials[i].MaterialName;
                        
                        if (root.Materials[i].WarnStatus == "1")
                        {
                            dataGridView1.Rows[i].Cells[1].Value = "换箱扫码";
                            if (dataGridView1.Rows[i].Cells[1].Style.ForeColor == Color.Red)
                            {
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.White;
                            }
                            else
                            {
                                dataGridView1.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                            }
                            dataGridView1.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 18, FontStyle.Bold);
                        }
                        else
                        {
                            dataGridView1.Rows[i].Cells[1].Value = "";
                        }

                    }

                    if (root.Parameters != null)
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
                                dataGridView2.Rows[i].Cells[7].Style.ForeColor = Color.Navy;
                                dataGridView2.Rows[i].Cells[7].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            }
                            if (i % 2 == 1)
                            {
                                dataGridView2.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                            }
                            dataGridView2.Rows[i].Cells[0].Value = root.Parameters[i * 3].ParameterName;
                            dataGridView2.Rows[i].Cells[1].Value = root.Parameters[i * 3].Data;
                            if (root.Parameters[i * 3].Status == "0")
                            {
                                dataGridView2.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                            }
                            else
                            {
                                dataGridView2.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            }
                            dataGridView2.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            dataGridView2.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                            dataGridView2.Rows[i].Cells[2].Value = root.Parameters[i * 3].Spc;
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
                                dataGridView2.Rows[i].Cells[5].Value = root.Parameters[i * 3 + 1].Spc;
                            }
                            if (i * num + 2 < root.Parameters.Count)
                            {
                                dataGridView2.Rows[i].Cells[6].Value = root.Parameters[i * num + 2].ParameterName;
                                dataGridView2.Rows[i].Cells[7].Value = root.Parameters[i * num + 2].Data; ;
                                if (root.Parameters[i * num + 2].Status == "0")
                                {
                                    dataGridView2.Rows[i].Cells[7].Style.BackColor = Color.LimeGreen;
                                }
                                else
                                {
                                    dataGridView2.Rows[i].Cells[7].Style.BackColor = Color.Red;
                                }
                                dataGridView2.Rows[i].Cells[7].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);

                                dataGridView2.Rows[i].Cells[8].Value = root.Parameters[i * num + 2].Spc;
                            }
                        }

                        //for (int i = (int)Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(3)); i < dataGridView2.Rows.Count; i++)
                        //{
                        //    dataGridView2.Rows.RemoveAt(i);
                        //}
                    }
                    else
                    {
                        groupBox1.Visible = false;
                    }
                }
                else
                {
                    int num = 2;
                    pictureBox10.Visible = true;
                    pictureBox10.Image = Properties.Resources.图片8;

                    for (int i = 0; i < root.Materials.Count; i++)
                    {
                        if (i >= dataGridView4.Rows.Count)
                        {
                            dataGridView4.Rows.Add();
                        }
                        dataGridView4.Rows[i].Cells[0].Value = root.Materials[i].MaterialName;
                        //dataGridView4.Rows[i].Cells[1].Value = Convert.ToSingle(root.Materials[i].Percentage);
                        if (root.Materials[i].WarnStatus == "1")
                        {
                            dataGridView4.Rows[i].Cells[1].Value = "换箱扫码";
                            if (dataGridView4.Rows[i].Cells[1].Style.ForeColor == Color.Red)
                            {
                                dataGridView4.Rows[i].Cells[1].Style.ForeColor = Color.Black;
                            }
                            else
                            {
                                dataGridView4.Rows[i].Cells[1].Style.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            dataGridView4.Rows[i].Cells[1].Value = "";
                        }
                        dataGridView4.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 18, FontStyle.Bold);
                    }

                    

                    if (root.Parameters != null)
                    {
                        for (int i = 0; i < Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(3)); i++)
                        {
                            if (i >= dataGridView3.Rows.Count)
                            {
                                dataGridView3.Rows.Add();
                                dataGridView3.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                                dataGridView3.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            }
                            if (i % 2 == 1)
                            {
                                dataGridView3.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;
                            }
                            dataGridView3.Rows[i].Cells[0].Value = root.Parameters[i].ParameterName;
                            dataGridView3.Rows[i].Cells[1].Value = root.Parameters[i].Data;
                            dataGridView3.Rows[i].Cells[1].Style.Font = new Font("微软雅黑", 22, FontStyle.Bold);
                            dataGridView3.Rows[i].Cells[1].Style.ForeColor = Color.Navy;
                            if (root.Parameters[i].Status == "1")
                            {
                                dataGridView3.Rows[i].Cells[1].Style.BackColor = Color.LimeGreen;
                            }
                            else
                            {
                                dataGridView3.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            }
                            dataGridView3.Rows[i].Cells[2].Value = root.Parameters[i].Spc;
                        }
                        
                        //for (int i = (int)Math.Ceiling(Convert.ToSingle(root.Parameters.Count) / Convert.ToSingle(3)); i < dataGridView2.Rows.Count; i++)
                        //{
                        //    dataGridView2.Rows.RemoveAt(i);
                        //}
                    }
                    else
                    {
                        groupBox1.Visible = false;
                    }
                }
                
                int cnt = 1;

                for (int i = 0; i < root.TitleHeads.Count; i++)
                {
                    if (i < 3)
                    {
                        var temp = titleHeads.Where(t => t.Title == root.TitleHeads[i].Title).FirstOrDefault();
                        if (temp != null)
                        {
                            ActiveMatch(cnt++, root.TitleHeads[i].Title, root.TitleHeads[i].Status,serial);
                        }
                    }
                    else
                    {
                        ActiveMatch(i + 1, root.TitleHeads[i].Title, root.TitleHeads[i].Status,serial);
                    }
                }
            }
            else
            {
                if (serial == 1)
                {
                    pictureBox3.Visible = true;
                    pictureBox3.Image = Properties.Resources.图片8;
                }
                else
                {
                    pictureBox10.Visible = true;
                    pictureBox10.Image = Properties.Resources.图片8;
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                ControlRefresh(1);
            }
            catch 
            {
                pictureBox6.Image = Properties.Resources.图片5;
            }
            label2.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Form2_SizeChanged(object sender, EventArgs e)
        {
            //asc.ControlAutoSize(this);
        }

        private void Timer2_Tick(object sender, EventArgs e)
        {
            try
            {

                ControlRefresh(2);
            }
            catch (Exception ex)
            {
                //pictureBox10.Image = Properties.Resources.图片5;
                logger.Error("读取API数据异常" + ex.Message);
            }
        }

        private void ActiveMatch(int num, string text, string value,int serial)
        {
            switch (num)
            {

                case 1://打印机
                    if (serial == 1)
                    {
                        label8.Text = text;
                        pictureBox4.Visible = true;

                        if (value != "0")
                            pictureBox4.Image = Properties.Resources.图片8;
                        else
                            pictureBox4.Image = Properties.Resources.图片5;
                    }
                    else
                    {
                        label17.Text = text;
                        pictureBox9.Visible = true;

                        if (value != "0")
                            pictureBox9.Image = Properties.Resources.图片8;
                        else
                            pictureBox9.Image = Properties.Resources.图片5;
                    }
                    break;
                case 2://PLC
                    if (serial == 1)
                    {
                        label9.Text = text;
                        pictureBox5.Visible = true;

                        if (value != "0")
                            pictureBox5.Image = Properties.Resources.图片8;
                        else
                            pictureBox5.Image = Properties.Resources.图片5;

                    }
                    else
                    {
                        label14.Text = text;
                        pictureBox8.Visible = true;

                        if (value != "0")
                            pictureBox8.Image = Properties.Resources.图片8;
                        else
                            pictureBox8.Image = Properties.Resources.图片5;
                    }
                    break;
                case 3://物料扫码枪
                    if (serial == 1)
                    {
                        label11.Text = text;
                        pictureBox6.Visible = true;

                        if (value != "0")
                            pictureBox6.Image = Properties.Resources.图片8;
                        else
                            pictureBox6.Image = Properties.Resources.图片5;
                    }
                    else
                    {
                        label16.Text = text;
                        pictureBox7.Visible = true;

                        if (value != "0")
                            pictureBox7.Image = Properties.Resources.图片8;
                        else
                            pictureBox7.Image = Properties.Resources.图片5;
                    }
                    
                    break;
                case 4://条码
                    if (serial == 1)
                    {
                        textBox6.Text = value;
                    }
                    else
                    {
                        textBox10.Text = value;
                    }
                    
                    break;
                case 5://当前物料
                    if (serial == 1)
                    {
                        textBox1.Text = value.Split('/')[1];
                        textBox7.Text = value.Split('/')[0]; ;
                    }
                    else
                    {
                        textBox2.Text = value.Split('/')[1];
                        textBox9.Text = value.Split('/')[0]; ;
                    }
                    
                    break;
                case 6://扫码结果
                    if (serial == 1)
                    {
                        textBox8.Text = text;
                    }
                    else
                    {
                        textBox3.Text = text;
                    }
                    
                    break;
                case 7://车型
                    if (serial == 1)
                    {
                        textBox5.Text = value;
                    }
                    else
                    {
                        textBox11.Text = value;
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
