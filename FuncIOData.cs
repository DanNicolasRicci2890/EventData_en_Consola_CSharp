using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PCD_ColorFull;

namespace PCD_EVENT_DATA
{
    public interface FuncIOData
    {
        void Display(color back, color fore);
        object GetDataInfo();
        void SetDataInfo(object dataInfo);
        void SetTypeDataIN(TypeDataIN cond);
    }
}
