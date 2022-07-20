using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HeadUpDsiplay
{
    public class AutoSizeFormClass
    {
        /// <summary>
        /// 声明结构 记录控件位置和大小
        /// </summary>
        public struct ControlRect
        {
            public int Left;
            public int Top;
            public int Width;
            public int Height;
            public float Size;
        }

        public List<ControlRect> _oldCtrl = new List<ControlRect>();
        private int _ctrlNo = 0;

        private void AddControl(Control ctrl)
        {
            foreach (Control c in ctrl.Controls)
            {
                ControlRect cR;
                cR.Left = c.Left;
                cR.Top = c.Top;
                cR.Width = c.Width;
                cR.Height = c.Height;
                cR.Size = c.Font.Size;
                _oldCtrl.Add(cR);
                // 控件可能嵌套子控件
                if (c.Controls.Count > 0)
                    AddControl(c);
            }
        }

        public void ControlAutoSize(Control mForm)
        {
            if (_ctrlNo == 0)
            {
                ControlRect cR;
                cR.Left = mForm.Left;
                cR.Top = mForm.Top;
                cR.Width = mForm.Width;
                cR.Height = mForm.Height;
                cR.Size = mForm.Font.Size;
                _oldCtrl.Add(cR);

                AddControl(mForm);
            }

            _ctrlNo = 1;
            float wScale = (float)mForm.Width / _oldCtrl[0].Width;
            float hScale = (float)mForm.Height / _oldCtrl[0].Height;
            AutoScaleControl(mForm, wScale, hScale);
        }

        private void AutoScaleControl(Control mForm, float wScale, float hScale)
        {
            int ctrlLeft, ctrlTop, ctrlWidth, ctrlHeight;
            float ctrlFontSize, hSize, wSize;
            foreach (Control c in mForm.Controls)
            {
                ctrlLeft = _oldCtrl[_ctrlNo].Left;
                ctrlTop = _oldCtrl[_ctrlNo].Top;
                ctrlWidth = _oldCtrl[_ctrlNo].Width;
                ctrlHeight = _oldCtrl[_ctrlNo].Height;
                ctrlFontSize = _oldCtrl[_ctrlNo].Size;

                c.Left = (int)Math.Ceiling(ctrlLeft * wScale);
                c.Top = (int)Math.Ceiling(ctrlTop * hScale);
                c.Width = (int)Math.Ceiling(ctrlWidth * wScale);
                c.Height = (int)Math.Ceiling(ctrlHeight * hScale);

                wSize = ctrlFontSize * wScale;
                hSize = ctrlFontSize * hScale;
                c.Font = new Font(c.Font.Name, Math.Min(hSize, wSize), c.Font.Style, c.Font.Unit);

                if (c is DataGridView)
                {
                    DataGridView dt = (DataGridView)c;
                    //xxdt.RowsDefaultCellStyle.Font = new Font(c.Font.Name, Math.Min(hSize, wSize), c.Font.Style, c.Font.Unit);

                }

                _ctrlNo++;

                // 先缩放控件本身 再缩放子控件
                if (c.Controls.Count > 0)
                {
                    AutoScaleControl(c, wScale, hScale);
                }
            }
        }
    }
}
