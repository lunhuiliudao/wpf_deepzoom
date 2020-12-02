using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFDeepZoom
{
    public class SlideListItem
    {
        private string _strFileName;

        private ImageSource _imgSourceLabel;

        private ImageSource _imgSourceThumbnail;

        public string strFileName
        {
            get
            {
                return _strFileName;
            }
            set
            {
                _strFileName = value;
            }
        }

        public ImageSource imgSourceLabel
        {
            get
            {
                return _imgSourceLabel;
            }
            set
            {
                _imgSourceLabel = value;
            }
        }

        public ImageSource imgSourceThumbnail
        {
            get
            {
                return _imgSourceThumbnail;
            }
            set
            {
                _imgSourceThumbnail = value;
            }
        }
    }

}
