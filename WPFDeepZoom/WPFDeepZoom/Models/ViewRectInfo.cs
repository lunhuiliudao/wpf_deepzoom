using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFDeepZoom
{
    public class ViewRectInfo
    {
        public Point pRatioLeftTop;

        private float _fXMotorPos;

        public float _fYMotorPos;

        private float _fZMotorPos;

        private string _strAutoFocus;

        public float fXMotorPos
        {
            get
            {
                return _fXMotorPos;
            }
            set
            {
                _fXMotorPos = value;
            }
        }

        public float fYMotorPos
        {
            get
            {
                return _fYMotorPos;
            }
            set
            {
                _fYMotorPos = value;
            }
        }

        public float fZMotorPos
        {
            get
            {
                return _fZMotorPos;
            }
            set
            {
                _fZMotorPos = value;
            }
        }

        public string strAutoFocus
        {
            get
            {
                return _strAutoFocus;
            }
            set
            {
                _strAutoFocus = value;
            }
        }
    }

}
